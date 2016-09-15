using Composr.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.AspNetCore.Http;
using System.Text;

namespace Composr.Web.Models
{
    public class SitemapResult : ActionResult
    {
        private SitemapModel sitemap;

        public SitemapResult(SitemapModel sitemap)
        {
            this.sitemap = sitemap;
        }

        public override void ExecuteResult(ActionContext context)
        {
            var response = context.HttpContext.Response;
            response.ContentType = "application/xml";
            WriteToStream(response);
        }
        
        private void WriteToStream(HttpResponse response)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(SitemapModel));
            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            using (XmlWriter writer = new XmlTextWriter(response.Body, Encoding.UTF8))
            {
                ns.Add("xhtml", "http://www.w3.org/1999/xhtml");
                ns.Add("", "http://www.sitemaps.org/schemas/sitemap/0.9");
                serializer.Serialize(writer, sitemap, ns);
            }
        }
    }
}
