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
                AdPublisherJSCode = model.AdPublisherJSCode,
                CategoryPages = model.CategoryPages
            };
        }

        public IList<Hit> SearchResults { get; set; }
        public string SearchQuery { get; set; }
        public int PageCount { get; internal set; }
        public int CurrentPage { get; internal set; }
        public string SearchUrl { get; set; }
        public string SearchCategory { get; set; }
        public IList<Hit> RelatedResults { get; set; }
    }
}
