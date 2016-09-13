using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;

namespace Composr.Web.ViewModels
{
    public class BaseFrontEndViewModel
    {
        public BaseFrontEndViewModel()
        {
            Breadcrumbs = new List<ViewModels.Breadcrumb>();
            HrefLangUrls = new Dictionary<string, string>();
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
        //private string referrer;
        public string Referrer { get; set; }
        public List<Breadcrumb> Breadcrumbs { get; set; }        
        public Dictionary<string, string> HrefLangUrls { get; set; }
    }

    public class Breadcrumb
    {
        public bool IsActive { get; set; }
        public string Url { get; set; }
        public string Name { get; set; }
    }

}
