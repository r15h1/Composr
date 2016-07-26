using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Composr.Web.ViewModels
{
    [Serializable]
    [XmlRoot("urlset", Namespace = "http://www.sitemaps.org/schemas/sitemap/0.9")]
    public class SitemapModel
    {        
        public SitemapModel()
        {
            Urls = new List<ViewModels.SiteUrl>();
        }

        [XmlElement("url")]
        public List<SiteUrl> Urls { get; set; }
    }

    public class SiteUrl
    {
        [XmlElement("loc")]
        public string Location { get; set; }        
    }
}
