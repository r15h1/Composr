using Composr.Core;
using Composr.Lib.Util;
using Composr.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace Composr.Web.Controllers
{
    public class FrontEndController : BaseFrontEndController
    {
        private ISearchService service;
        private IRedirectionMapper redirectionMapper;

        public FrontEndController(ISearchService service, IRedirectionMapper redirectionMapper, Blog blog) :base(blog)
        {
            this.service = service;
            this.redirectionMapper = redirectionMapper;
        }

        // GET: /<controller>/
        [HttpGet]
        public IActionResult Index()
        {
            var results = service.Search(new SearchCriteria() { BlogID = Blog.Id.Value, Locale = Blog.Locale.Value, SearchSortOrder = SearchSortOrder.MostRecent, Limit=12 });
            var model = PostSearchViewModel.FromBaseFrontEndViewModel(BaseViewModel);
            model.SearchResults = results;
            model.Title = $"{Blog.Name} - {Blog.Attributes[BlogAttributeKeys.Tagline]}";
            return View(model);
        }

        [HttpGet]
        public IActionResult FindPost(string postkey)
        {
            var results = service.Search(new SearchCriteria() { BlogID = Blog.Id.Value, Locale = Blog.Locale.Value, URN = HttpContext.Request.Path.Value});

            if (results != null && results.Count > 0)
                return GetPostDetails(results);
            else if(redirectionMapper.CanResolve(postkey))
                return RedirectPermanent(redirectionMapper.MapToRedirectUrl(postkey));

            return NotFound();
        }

        private IActionResult GetPostDetails(IList<SearchResult> results)
        {
            var model = PostSearchViewModel.FromBaseFrontEndViewModel(BaseViewModel);
            model.Title = $"{results[0].Title} - {Blog.Name}";
            model.MetaDescription = $"{results[0].MetaDescription}";
            model.CanonicalUrl = $"{model.BlogUrl.TrimEnd('/')}{results[0].URN}";
            model.SearchResults = results;
            return View("PostDetails", model);
        }

        [HttpGet("api/autocomplete")]
        public IActionResult AutoComplete(string q)
        {
            var results = service.Search(new SearchCriteria() { BlogID = Blog.Id.Value, Locale = Blog.Locale.Value, SearchSortOrder = SearchSortOrder.BestMatch, Limit = 5, SearchTerm = q, SearchType = SearchType.AutoComplete });
            if (results.Count > 0) results.Add(new SearchResult { Title = "Display all results", URN=$"/search?q={q}" });
            return new JsonResult(new { suggestions = results.Select(r => new { value = r.Title, data = r.URN }) });
        }

        public IActionResult Search(string q)
        {
            var results = service.Search(new SearchCriteria() { BlogID = Blog.Id.Value, Locale = Blog.Locale.Value, SearchSortOrder = SearchSortOrder.BestMatch, Limit = 50, SearchTerm = q, SearchType = SearchType.Search });
            var model = PostSearchViewModel.FromBaseFrontEndViewModel(BaseViewModel);
            model.SearchResults = results;
            model.SearchQuery = q;
            model.Title = $"Search Results for {q} - Cocozil";
            model.CanonicalUrl = $"{model.BlogUrl.TrimEnd('/')}/search?q={System.Net.WebUtility.UrlEncode(q)}";
            return View(model);
        }

        public IActionResult Error(string error)
        {
            var model = new BaseFrontEndViewModel()
            {
                BlogUrl = Blog.Url,
                LogoUrl = Blog.Attributes[BlogAttributeKeys.LogoUrl],
                MetaDescription = "The page you are looking for does not exist. You will be redirected to the home page shortly.",
                Title = "Page Not Found - Cocozil",
                CanonicalUrl = null
            };
            return View(model);
        }
    }
}