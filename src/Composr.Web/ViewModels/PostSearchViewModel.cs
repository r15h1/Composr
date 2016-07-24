using Composr.Core;
using System.Collections.Generic;

namespace Composr.Web.ViewModels
{
    public class PostSearchViewModel:BaseFrontEndViewModel
    {
        public static PostSearchViewModel FromBaseFrontEndViewModel(BaseFrontEndViewModel model)
        {
            return new PostSearchViewModel()
            {
                BlogUrl = model.BlogUrl,
                Copyright = model.Copyright,
                LogoUrl = model.LogoUrl,
                Tagline = model.Tagline,
                CanonicalUrl = model.CanonicalUrl,
                AnalyticsTrackingJSCode = model.AnalyticsTrackingJSCode,
                AdPublisherJSCode = model.AdPublisherJSCode
            };
        }

        public IList<SearchResult> SearchResults { get; set; }

        public string SearchQuery { get; set; }
    }
}
