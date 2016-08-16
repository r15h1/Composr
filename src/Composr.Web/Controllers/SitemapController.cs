using Composr.Core;
using Composr.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace Composr.Web.Controllers
{
    public class SitemapController
    {
        private Blog blog;
        private ISearchService service;

        public SitemapController(Blog blog, ISearchService service)
        {
            this.blog = blog;
            this.service = service;
        }

        [Produces("application/xml")]
        public SitemapModel Index()
        {
            var results = service.Search(new SearchCriteria() { BlogID = blog.Id.Value, Locale = blog.Locale.Value, SearchSortOrder = SearchSortOrder.MostRecent, Limit = 1000 });
            SitemapModel sitemap = new ViewModels.SitemapModel();
            sitemap.Urls = results.Hits.Select(r => new SiteUrl { Location = $"{blog.Url}{r.URN}" }).ToList();
            return sitemap;
        }
    }
}
