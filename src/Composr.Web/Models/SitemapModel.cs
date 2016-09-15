using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Composr.Web.Models
{
    [Serializable]
    [XmlRoot(ElementName = "urlset", Namespace = "http://www.sitemaps.org/schemas/sitemap/0.9")]
    public class SitemapModel
    {        
        public SitemapModel()
        {
            Urls = new List<SiteUrl>();
        }

        [XmlElement(ElementName = "url")]
        public List<SiteUrl> Urls { get; set; }
    }

    
    public class SiteUrl
    {
        [XmlElement("loc")]
        public string Location { get; set; }

        [XmlElement(ElementName = "link", Namespace = "http://www.w3.org/1999/xhtml")]
        public List<HrefLang> HrefLangs { get; set; }
    }

    public class HrefLang
    {
        public HrefLang()
        {
            Rel = "alternate";
        }

        [XmlAttribute("hreflang")]
        public string Locale { get; set; }

        [XmlAttribute("href")]
        public string Url { get; set; }

        [XmlAttribute("rel")]
        public string Rel { get; set; }
    }
}
