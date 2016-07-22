using Composr.Core;
using System.Collections.Concurrent;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System;

namespace Composr.Lib.Util
{
    public class RedirectionMapper:IRedirectionMapper
    {
        private ConcurrentDictionary<string, string> redirections;

        public RedirectionMapper(IOptions<List<RedirectionMapping>> mappings)
        {
            Initialize(mappings);
        }

        private void Initialize(IOptions<List<RedirectionMapping>> mappings)
        {
            redirections = new ConcurrentDictionary<string, string>();
            foreach (var mapping in mappings.Value)
                if (!string.IsNullOrWhiteSpace(mapping.From) && !string.IsNullOrWhiteSpace(mapping.To))
                    redirections.TryAdd(mapping.From, mapping.To);
        }

        public bool CanResolve(string url)
        {
            return redirections.ContainsKey(url);
        }

        public string MapToRedirectUrl(string url)
        {
            if (redirections.ContainsKey(url))
                return redirections[url];

            return null;
        }        
    }

    public class RedirectionMapping
    {
        public string From { get; set; }
        public string To { get; set; }
    }
}
