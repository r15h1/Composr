﻿using Composr.Core;
using Composr.Lib.Util;
using Composr.Web.Models;
using Composr.Web.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Composr.Web.Controllers
{
    public class FrontEndController : BaseFrontEndController
    {
        private ISearchService service;
        private IRedirectionMapper redirectionMapper;
        private IStringLocalizer<FrontEndController> localizer;

        public FrontEndController(ISearchService service, IRedirectionMapper redirectionMapper, Blog blog, IStringLocalizer<FrontEndController> localizer) : base(blog)
        {
            this.service = service;
            this.redirectionMapper = redirectionMapper;
            this.localizer = localizer;
        }

        // GET: /<controller>/git
        [HttpGet]
        public IActionResult Index(SearchParameters param)
        {
            var model = GetViewModel(param, SearchSortOrder.MostRecent);
            model.Title = $"{Blog.Attributes[BlogAttributeKeys.Tagline]} - {Blog.Name}";
            model.CanonicalUrl = model.CurrentPage <= 1 ? $"{model.BlogUrl.TrimEnd('/')}" : $"{model.BlogUrl.TrimEnd('/')}?page={model.CurrentPage}";
            model.SearchUrl = null;
            return View(model);
        }

        [HttpGet]
        public IActionResult FindPost(string postkey)
        {
            var results = service.Search(new SearchCriteria() { BlogID = Blog.Id.Value, Locale = Blog.Locale.Value, SearchTerm = HttpContext.Request.Path.Value, SearchType = SearchType.URN });

            if (results != null && results.Hits.Count > 0)
                return GetPostDetails(results.Hits);
            else if (redirectionMapper.CanResolve(postkey))
                return RedirectPermanent(redirectionMapper.MapToRedirectUrl(postkey));

            return NotFound();
        }

        private IActionResult GetPostDetails(IList<Hit> results)
        {
            var model = PostSearchViewModel.FromBaseFrontEndViewModel(BaseViewModel);
            model.Referrer = GetReferrer();
            UpdateBreadcrumbs(model, GetDetailBreadCrumbs(model));
            model.Title = $"{results[0].Title} - {Blog.Name}";
            model.MetaDescription = $"{results[0].MetaDescription}";
            model.CanonicalUrl = $"{model.BlogUrl.TrimEnd('/')}{results[0].URN}";
            model.SearchResults = results;
            return View("PostDetails", model);
        }

        private List<Breadcrumb> GetDetailBreadCrumbs(PostSearchViewModel model)
        {
            List<Breadcrumb> crumbs = new List<Breadcrumb> { new Breadcrumb { Name = localizer["Home"], Url = "/" } };
            if (!string.IsNullOrWhiteSpace(model.Referrer) && model.Referrer.ToLower().Contains("/search"))
                crumbs.Add(new Breadcrumb { Name = localizer["Search Results"], Url = $"/{model.Referrer.ToLowerInvariant().Replace(model.BlogUrl.ToLowerInvariant(), string.Empty).TrimStart(new char[] { '/' }) }" });

            crumbs.Add(new Breadcrumb { IsActive = true, Name = localizer["Recipe"] });
            return crumbs;
        }


        [HttpGet("api/autocomplete")]
        public IActionResult AutoComplete(string q)
        {
            var results = service.Search(new SearchCriteria() { BlogID = Blog.Id.Value, Locale = Blog.Locale.Value, SearchSortOrder = SearchSortOrder.BestMatch, Limit = 5, SearchTerm = q, SearchType = SearchType.AutoComplete });
            if (results.Hits.Count > 0) results.Hits.Add(new Hit { Title = "Display all results", URN = $"/search?q={q}" });
            return new JsonResult(new { suggestions = results.Hits.Select(r => new { value = r.Title, data = r.URN }) });
        }

        public IActionResult Search(SearchParameters param)
        {
            var model = GetViewModel(param, SearchSortOrder.BestMatch);
            model.Referrer = GetReferrer();
            UpdateBreadcrumbs(model, new List<Breadcrumb>{
                new Breadcrumb { Name = localizer["Home"], Url = "/" },
                new Breadcrumb { IsActive = true, Name = localizer["Search Results"] }
            });

            model.Title = $"{(string.IsNullOrWhiteSpace(param.Query) ? localizer["tag"] + ": " + param.Category : param.Query)} - {localizer["Cocozil Search"]}";
            model.CanonicalUrl = $"{model.BlogUrl.TrimEnd('/')}/search?q={System.Net.WebUtility.UrlEncode(param.Query)}&page={model.CurrentPage}";
            return View(model);
        }

        private static void UpdateBreadcrumbs(PostSearchViewModel model, List<Breadcrumb> breadcrumbs)
        {
            breadcrumbs.ForEach((breadcrumb) =>
            {
                if (model.Breadcrumbs.Any(b => b.Name.Equals(breadcrumb.Name)))
                    model.Breadcrumbs.Remove(model.Breadcrumbs.SingleOrDefault(b => b.Name.Equals(breadcrumb.Name)));

                model.Breadcrumbs.Add(breadcrumb);
            });
        }

        private PostSearchViewModel GetViewModel(SearchParameters param, SearchSortOrder sort)
        {
            var criteria = GetSearchCriteria(param);
            criteria.SearchSortOrder = sort;
            var results = service.Search(criteria);
            var model = PostSearchViewModel.FromBaseFrontEndViewModel(BaseViewModel);
            model.SearchResults = results.Hits;
            model.SearchQuery = param.Query;
            model.SearchCategory = param.Category;
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
                MetaDescription = localizer["The page you are looking for does not exist. You will be redirected to the home page shortly."],
                Title = localizer["Page Not Found (404) - Cocozil"],
                CanonicalUrl = null
            };
            return View(model);
        }

        [HttpPost]
        public IActionResult Translate(string sourceLanguage, string targetLanguage, string url)
        {
            var returnUrl = FindTranslatedPost(HttpContext.Request.Path, sourceLanguage, targetLanguage);
            return FindPost(returnUrl);
        }

        private string FindTranslatedPost(PathString path, string sourceLanguage, string targetLanguage)
        {
            throw new NotImplementedException();
        }
    }
}