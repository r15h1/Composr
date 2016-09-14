using System;
using System.Collections.Generic;
using Microsoft.Extensions.Localization;
using Composr.Core;
using System.Globalization;

namespace Composr.Web.Middleware
{
    public class HrefLangLocalizerCollection
    {
        private IStringLocalizer localizer;
        private Locale currentLocale;
        public HrefLangLocalizerCollection(Locale currentLocale, IStringLocalizer localizer)
        {
            this.localizer = localizer;
            this.currentLocale = currentLocale;
        }

        public IEnumerable<HrefLangLocalizer> GetAlternateLocalizers()
        {
            foreach(var locale in Enum.GetValues(typeof(Locale)))
                yield return new HrefLangLocalizer((Locale) locale, localizer.WithCulture(new CultureInfo(((Locale)locale).ToString())));
        }
    }

    public class HrefLangLocalizer
    {
        public HrefLangLocalizer (Locale locale, IStringLocalizer localizer)
        {
            Localizer = localizer;
            Locale = locale;
        }

        public IStringLocalizer Localizer { get; private set; }
        public Locale Locale { get; private set; }
    }
}
