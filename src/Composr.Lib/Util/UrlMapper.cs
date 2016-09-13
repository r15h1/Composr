using Composr.Core;
using System.Collections.Concurrent;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System;
using System.Linq;

namespace Composr.Lib.Util
{
    public class UrlMapper:IUrlMapper
    {
        private ConcurrentDictionary<string, string> redirections;
        private ConcurrentDictionary<TranslationMapping, IList<TranslationMapping>> translations;

        public UrlMapper(IOptions<List<RedirectionMapping>> mappings)
        {
            InitializeRedirections(mappings);
            InitializaTranslations();
        }

        private void InitializaTranslations()
        {
            translations = new ConcurrentDictionary<TranslationMapping, IList<TranslationMapping>>(new TranslationsComparer());
            translations.TryAdd(
                new TranslationMapping { Locale = Locale.EN, Url = "" },
                new List<TranslationMapping> { new TranslationMapping { Locale = Locale.FR, Url = "/fr" } }
            );

            translations.TryAdd(
                new TranslationMapping { Locale = Locale.EN, Url = "/" },
                new List<TranslationMapping> { new TranslationMapping { Locale = Locale.FR, Url = "/fr" } }
            );

            translations.TryAdd(
                new TranslationMapping { Locale = Locale.EN, Url = "/en" },
                new List<TranslationMapping> { new TranslationMapping { Locale = Locale.FR, Url = "/fr" } }
            );

            translations.TryAdd(
                new TranslationMapping { Locale = Locale.EN, Url = "/en/search" },
                new List<TranslationMapping> { new TranslationMapping { Locale = Locale.FR, Url = "/fr/rechercher" } }
            );

            translations.TryAdd(
                new TranslationMapping { Locale = Locale.FR, Url = "/fr" },
                new List<TranslationMapping> { new TranslationMapping { Locale = Locale.EN, Url = "/" } }
            );

            translations.TryAdd(                
                new TranslationMapping { Locale = Locale.FR, Url = "/fr/rechercher" },
                new List<TranslationMapping> { new TranslationMapping { Locale = Locale.EN, Url = "/en/search" } }
            );
        }

        private void InitializeRedirections(IOptions<List<RedirectionMapping>> mappings)
        {
            redirections = new ConcurrentDictionary<string, string>();
            foreach (var mapping in mappings.Value)
                if (!string.IsNullOrWhiteSpace(mapping.From) && !string.IsNullOrWhiteSpace(mapping.To))
                    redirections.TryAdd(mapping.From, mapping.To);
        }

        public bool HasRedirectUrl(string url)
        {
            return !string.IsNullOrWhiteSpace(url) && redirections.ContainsKey(url);
        }

        public string GetRedirectUrl(string url)
        {
            if (redirections.ContainsKey(url))
                return redirections[url];

            return null;
        }

        public bool HasTranslatedUrl(Locale sourceLocale, Locale targetLocale, string url)
        {
            IList<TranslationMapping> mappings;
            if (translations.TryGetValue(new TranslationMapping { Locale = sourceLocale, Url = url }, out mappings))
                return mappings != null && mappings.Any(m => m.Locale == targetLocale);                    

            return false;
        }

        public string GetTranslatedUrl(Locale sourceLocale, Locale targetLocale, string url)
        {
            IList<TranslationMapping> mappings;
            if (translations.TryGetValue(new TranslationMapping { Locale = sourceLocale, Url = url }, out mappings))
                if (mappings != null && mappings.Any(m => m.Locale == targetLocale))
                    return mappings.FirstOrDefault(m => m.Locale == targetLocale).Url;

            return string.Empty;
        }
    }

    public class RedirectionMapping
    {
        public string From { get; set; }
        public string To { get; set; }
    }

    public class TranslationMapping
    {
        public Locale Locale { get; set; }
        public string Url { get; set; }
    }

    public class TranslationsComparer : EqualityComparer<TranslationMapping>
    {
        public override bool Equals(TranslationMapping x, TranslationMapping y)
        {
            return x.Locale == y.Locale &&
                        (
                            (string.IsNullOrWhiteSpace(x.Url) && string.IsNullOrWhiteSpace(y.Url)) ||
                            (x.Url.ToLowerInvariant().Equals(y.Url.ToLowerInvariant()))
                        );
        }

        public override int GetHashCode(TranslationMapping obj)
        {
            return obj.Locale.GetHashCode() + (string.IsNullOrWhiteSpace(obj.Url) ? 0 : obj.Url.GetHashCode());
        }
    }
}
