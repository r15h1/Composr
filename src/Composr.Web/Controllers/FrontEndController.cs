using Composr.Core;
using Composr.Lib.Util;
using Composr.Web.Models;
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
        public IActionResult Index(SearchParameters param)
        {
            var model = GetViewModel(param, SearchSortOrder.MostRecent);
            model.Title = $"{Blog.Name} - {Blog.Attributes[BlogAttributeKeys.Tagline]}";
            model.CanonicalUrl = model.CurrentPage <= 1? $"{model.BlogUrl.TrimEnd('/')}" : $"{model.BlogUrl.TrimEnd('/')}?page={model.CurrentPage}";
            model.SearchUrl = null;
            return View(model);
        }

        [HttpGet]
        public IActionResult FindPost(string postkey)
        {
            var results = service.Search(new SearchCriteria() { BlogID = Blog.Id.Value, Locale = Blog.Locale.Value, SearchTerm = HttpContext.Request.Path.Value, SearchType = SearchType.URN});

            if (results != null && results.Hits.Count > 0)
                return GetPostDetails(results.Hits);
            else if(redirectionMapper.CanResolve(postkey))
                return RedirectPermanent(redirectionMapper.MapToRedirectUrl(postkey));

            return NotFound();
        }

        private IActionResult GetPostDetails(IList<Hit> results)
        {
            var model = PostSearchViewModel.FromBaseFrontEndViewModel(BaseViewModel);
            model.Referrer = GetReferrer();
            model.Breadcrumbs.Add(new Breadcrumb { IsActive = true, Name = "Recipe" });
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
            if (results.Hits.Count > 0) results.Hits.Add(new Hit { Title = "Display all results", URN=$"/search?q={q}" });
            return new JsonResult(new { suggestions = results.Hits.Select(r => new { value = r.Title, data = r.URN }) });
        }

        public IActionResult Search(SearchParameters param)
        {
            var model = GetViewModel(param, SearchSortOrder.BestMatch);
            model.Referrer = GetReferrer();
            UpdateBreadcrumbs(model, new Breadcrumb { IsActive = true, Name = "Search Results" });
            model.Title = $"Search Results for {param.Query} - Cocozil";
            model.CanonicalUrl = $"{model.BlogUrl.TrimEnd('/')}/search?q={System.Net.WebUtility.UrlEncode(param.Query)}&page={model.CurrentPage}";
            return View(model);
        }

        private static void UpdateBreadcrumbs(PostSearchViewModel model, Breadcrumb breadcrumb)
        {
            if(model.Breadcrumbs.Any(b => b.Name.Equals(breadcrumb.Name)))
                model.Breadcrumbs.Remove(model.Breadcrumbs.SingleOrDefault(b => b.Name.Equals(breadcrumb.Name)));

            model.Breadcrumbs.Add(breadcrumb);
        }

        private PostSearchViewModel GetViewModel(SearchParameters param, SearchSortOrder sort)
        {
            var criteria = GetSearchCriteria(param);
            criteria.SearchSortOrder = sort;
            var results = service.Search(criteria);
            var model = PostSearchViewModel.FromBaseFrontEndViewModel(BaseViewModel);
            model.SearchResults = results.Hits;
            model.SearchQuery = param.Query;
            model.PageCount = (int)((results.HitsCount / Settings.DefaultSearchPageSize) + 1);
            model.CurrentPage = (int)((criteria.Start / Settings.DefaultSearchPageSize) + 1);
            model.SearchUrl = "search";
            return model;
        }

        private SearchCriteria GetSearchCriteria(SearchParameters param)
        {
            return new SearchCriteria()
            {
                BlogID = Blog.Id.Value,
                Limit = Settings.DefaultSearchPageSize,
                Locale = Blog.Locale.Value,
                SearchSortOrder = SearchSortOrder.BestMatch,
                SearchTerm = param.Query,
                SearchType = SearchType.Default,
                Tags = string.IsNullOrWhiteSpace(param.Category) ? Settings.SearchDefaultPostTag : param.Category,
                Start = param.Page.HasValue && param.Page.Value > 0 ? ((param.Page.Value - 1) * Settings.DefaultSearchPageSize) : 0
            };
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