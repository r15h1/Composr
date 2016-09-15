using Composr.Core;
using Composr.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System;
using Composr.Web.Models;

namespace Composr.Web.Controllers
{
    public class SitemapController
    {        
        private ISearchService service;
        private IBlogRepository repository;

        public SitemapController(ISearchService service, IBlogRepository repository)
        {
            this.service = service;
            this.repository = repository;
        }

        public IActionResult Index()
        {
            SitemapModel sitemap = new SitemapModel();
            List<Blog> blogs = new List<Blog>();
            foreach (var locale in Enum.GetValues(typeof(Locale))) blogs.AddRange(GetBlogs((Locale)locale));
            foreach (var blog in blogs) sitemap.Urls.AddRange(GetSitemap(blog));
            return new SitemapResult(sitemap);
        }

        private IList<SiteUrl> GetSitemap(Blog blog)
        {
            var results = service.Search(new SearchCriteria() { BlogID = blog.Id.Value, Locale = blog.Locale.Value, SearchSortOrder = SearchSortOrder.MostRecent, Limit = 1000 });
            return results.Hits.Select(r => BuildSiteUrls(blog, r)).ToList();
        }

        private IList<Blog> GetBlogs(Locale locale)
        {
            repository.Locale = locale;
            return repository.Get(null);
        }

        private SiteUrl BuildSiteUrls(Blog blog, Hit r)
        {
            var url = new SiteUrl { Location = $"{blog.Url}{r.URN}", HrefLangs = new List<HrefLang>() };
            url.HrefLangs.Add( new HrefLang { Locale = blog.Locale.ToString().ToLowerInvariant(), Url = url.Location });

            foreach(var translation in r.Translations)
                url.HrefLangs.Add( new HrefLang { Locale = translation.Key.ToString().ToLowerInvariant(), Url = $"{blog.Url}{translation.Value}"});

            return url;
        }
    }
}
