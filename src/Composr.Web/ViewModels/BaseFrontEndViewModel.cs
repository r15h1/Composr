using System;
using System.Collections.Generic;

namespace Composr.Web.ViewModels
{
    public class BaseFrontEndViewModel
    {
        public BaseFrontEndViewModel()
        {
            Breadcrumbs = new List<ViewModels.Breadcrumb>();
        }

        public string LogoUrl { get; set; }
        public string BlogUrl { get; set; }
        public string Copyright { get; set; }
        public string Tagline { get; set; }
        public string Title { get; set; }
        public string MetaDescription { get; set; }
        public string ImageLocation { get; set; }
        public string CanonicalUrl { get; set; }
        public string AnalyticsTrackingJSCode { get; set; }
        public string AdPublisherJSCode { get; set; }

        private string referrer;
        public string Referrer { get
            {
                return referrer;
            }
            set
            {
                referrer = value;
                UpdateBreadcrumbs();
            }
        }

        private void UpdateBreadcrumbs()
        {
            Breadcrumbs.Add(new Breadcrumb { Name = "Home", Url = "/" });

            if (!string.IsNullOrWhiteSpace(referrer) && referrer.ToLower().Contains("/search")) Breadcrumbs.Add(
                new Breadcrumb
                {
                    Name = "Search Results",
                    Url = $"/{referrer.ToLowerInvariant().Replace(BlogUrl.ToLowerInvariant(), string.Empty).TrimStart(new char[] { '/' }) }"
                });
        }

        public List<Breadcrumb> Breadcrumbs { get; set; }        
    }

    public class Breadcrumb
    {
        public bool IsActive { get; set; }
        public string Url { get; set; }
        public string Name { get; set; }
    }

}
