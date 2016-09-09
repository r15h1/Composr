using Composr.Lib.Util;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using System.Globalization;
using System.Threading.Tasks;

namespace Composr.Web.Middleware
{
    public class RouteCultureProvider : IRequestCultureProvider
    {
        private CultureInfo defaultCulture;

        public RouteCultureProvider(RequestCulture requestCulture)
        {
            this.defaultCulture = requestCulture.Culture;
        }

        public Task<ProviderCultureResult> DetermineProviderCultureResult(HttpContext httpContext)
        {
            var url = httpContext.Request.Path.ToString();
            var segments = url.ToLowerInvariant().Split(new char[] { '/' });
            if(segments.Length > 1)            
                foreach(var culture in Settings.SupportedCultures)
                    if(segments[1].Equals(culture.TwoLetterISOLanguageName))
                        return Task.FromResult<ProviderCultureResult>(new ProviderCultureResult(culture.TwoLetterISOLanguageName, culture.TwoLetterISOLanguageName));            

            return Task.FromResult<ProviderCultureResult>(new ProviderCultureResult(defaultCulture.TwoLetterISOLanguageName, defaultCulture.TwoLetterISOLanguageName));
        }
    }
}
