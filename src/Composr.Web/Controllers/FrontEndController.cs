using Composr.Core;
using Composr.Lib.Util;
using Composr.Web.Middleware;
using Composr.Web.Models;
using Composr.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Localization;
using System.Collections.Generic;
using System.Linq;
using System;

namespace Composr.Web.Controllers
{
    public class FrontEndController : BaseFrontEndController
    {
        private ISearchService service;
        private IUrlMapper urlMapper;
        private IStringLocalizer<FrontEndController> localizer;
        private HrefLangLocalizerCollection hreflanglocalizers;

        public FrontEndController(ISearchService service, IUrlMapper urlMapper, Blog blog, 
            IStringLocalizer<FrontEndController> localizer, IBlogRepository blogRepository, IMemoryCache cache
            ) : base(blog, blogRepository, cache)
        {
            this.service = service;
            this.urlMapper = urlMapper;
            this.localizer = localizer;
            hreflanglocalizers = new HrefLangLocalizerCollection(blog.Locale.Value, localizer);
        }

        // GET: /<controller>/git
        [HttpGet]
        public IActionResult Index(SearchParameters param)
        {
            var model = GetViewModel(param, SearchSortOrder.MostRecent);
            model.Title = $"{Blog.Attributes[BlogAttributeKeys.Tagline]}";
            model.CanonicalUrl = (model.CurrentPage <= 1 ? $"{model.BlogUrl.TrimEnd('/')}{localizer["/"]}" : $"{model.BlogUrl.TrimEnd('/')}{localizer["/"]}?page={model.CurrentPage}").TrimEnd('/');
            foreach (var hrefLangLocalizer in hreflanglocalizers.GetAlternateLocalizers())
                model.HrefLangUrls.Add(hrefLangLocalizer.Locale.ToString().ToLowerInvariant(), model.CurrentPage <= 1 ? $"{model.BlogUrl.TrimEnd('/')}{hrefLangLocalizer.Localizer["/"]}" : $"{model.BlogUrl.TrimEnd('/')}{hrefLangLocalizer.Localizer["/"]}?page={model.CurrentPage}");

            model.SearchUrl = $"{Blog.Url.TrimEnd('/')}";
            return View($"{BaseViewModel.ViewPrefix}Index", model);
        }

        [HttpGet]
        public IActionResult FindPost(string postkey)
        {
            if (string.IsNullOrWhiteSpace(postkey) || !postkey.StartsWith("/")) postkey = "/" + postkey;

            var results = service.Search(new SearchCriteria() { BlogID = Blog.Id.Value, Locale = Blog.Locale.Value, SearchTerm = postkey, SearchType = SearchType.URN });

            if (results != null && results.Hits.Count > 0)
            {
                var relatedhits = service.GetMoreLikeThis(new SearchCriteria() { BlogID = Blog.Id.Value, Locale = Blog.Locale.Value, SearchType = SearchType.MoreLikeThis, DocumentId = results.Hits[0].DocumentId, Limit = 4 });
                return GetPostDetails(results.Hits, relatedhits.Hits);
            }
            else if (urlMapper.HasRedirectUrl(postkey))
                return RedirectPermanent(urlMapper.GetRedirectUrl(postkey));

            return Error("");
        }

        private IActionResult GetPostDetails(IList<Hit> results, IList<Hit> related)
        {
            var model = PostSearchViewModel.FromBaseFrontEndViewModel(BaseViewModel);
            model.Referrer = GetReferrer();            
            model.Title = $"{results[0].Title}";
            model.MetaDescription = $"{results[0].MetaDescription}";
            model.CanonicalUrl = $"{model.BlogUrl.TrimEnd('/')}{results[0].URN}".TrimEnd('/');
            model.HrefLangUrls.Add(Blog.Locale.ToString().ToLowerInvariant(), model.CanonicalUrl);

            UpdateBreadcrumbs(model, GetDetailBreadCrumbs(model));
            foreach (var translation in results[0].Translations)
                model.HrefLangUrls.Add(translation.Key.ToString().ToLowerInvariant(), $"{model.BlogUrl.TrimEnd('/')}{translation.Value}");

            model.SearchResults = results;
            model.RelatedResults = related;
            return View($"{BaseViewModel.ViewPrefix}PostDetails", model);
        }

        private List<Breadcrumb> GetDetailBreadCrumbs(PostSearchViewModel model)
        {
            List<Breadcrumb> crumbs = new List<Breadcrumb> { new Breadcrumb { Name = localizer["Home"], Url = localizer["/"] } };

            if (IsSearchPage(model.Referrer))
                crumbs.Add(new Breadcrumb { Name = localizer["Search Results"], Url = $"/{model.Referrer.ToLowerInvariant().Replace(model.BlogUrl.ToLowerInvariant(), string.Empty).TrimStart(new char[] { '/' }) }" });
            else if (IsCategoryPage(model.Referrer))
            {
                var category = BaseViewModel.CategoryPages.FirstOrDefault(x => x.URN == model.Referrer.Replace(Blog.Url, ""));
                crumbs.Add(new Breadcrumb { Name = category.Title, Url = $"/{model.Referrer.ToLowerInvariant().Replace(model.BlogUrl.ToLowerInvariant(), string.Empty).TrimStart(new char[] { '/' }) }" });
            }

            crumbs.Add(new Breadcrumb { IsActive = true, Name = model.Title });
            return crumbs;
        }

        private bool IsCategoryPage(string referrer)
        {
            if (!string.IsNullOrWhiteSpace(referrer) && BaseViewModel != null && BaseViewModel.CategoryPages != null && BaseViewModel.CategoryPages.Count > 0)
                return BaseViewModel.CategoryPages.Any(x => x.URN == referrer.Replace(Blog.Url, ""));

            return false;
        }

        private bool IsSearchPage(string referrer)
        {
            return !string.IsNullOrWhiteSpace(referrer) && referrer.ToLower().Contains(localizer["/en/search"]);
        }

        [HttpGet("api/autocomplete")]
        [HttpGet("{culture:regex(^en|fr$)?}/api/autocomplete")]
        public IActionResult AutoComplete(string q)
        {
            var results = service.Search(new SearchCriteria() { BlogID = Blog.Id.Value, Locale = Blog.Locale.Value, SearchSortOrder = SearchSortOrder.BestMatch, Limit = 5, SearchTerm = q, SearchType = SearchType.AutoComplete });
            if (results.Hits.Count > 0) results.Hits.Add(new Hit { Title = "Display all results", URN = $"{localizer["/en/search"]}?q={q}" });
            return new JsonResult(new { suggestions = results.Hits.Select(r => new { value = r.Title, data = r.URN }) });
        }

        public IActionResult Search(SearchParameters param)
        {
            var model = GetViewModel(param, SearchSortOrder.BestMatch);
            model.Referrer = GetReferrer();
            UpdateBreadcrumbs(model, new List<Breadcrumb>{
                new Breadcrumb { Name = localizer["Home"], Url = localizer["/"] },
                new Breadcrumb { IsActive = true, Name = localizer["Search Results"] }
            });

            model.Title = $"{localizer["Search Results"]} - {(string.IsNullOrWhiteSpace(param.Query) ? param.Category : param.Query)}".TrimEnd(new char[] { ' ', '-' });
            model.CanonicalUrl = $"{model.BlogUrl.TrimEnd('/')}{localizer["/en/search"]}{BuildQueryString(param)}".TrimEnd('/');
            model.HrefLangUrls.Add(Blog.Locale.ToString().ToLowerInvariant(), model.CanonicalUrl);

            return View($"{BaseViewModel.ViewPrefix}Search", model);
        }

        private string BuildQueryString(SearchParameters param)
        {
            string querystring = string.Empty;
            querystring = param.Page > 1 ? "page=" + param.Page.ToString() : string.Empty;
            querystring += (param.Query.IsBlank() ? string.Empty : (querystring.IsBlank() ? string.Empty : "&") + "q=" + param.Query);
            querystring += (param.Category.IsBlank() ? string.Empty : (querystring.IsBlank() ? string.Empty : "&") + "cat=" + param.Category);
            return querystring.IsBlank() ? string.Empty : $"?{querystring}";
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
            model.SearchUrl = $"{Blog.Url.TrimEnd('/')}{localizer["/en/search"]}";
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
                Title = localizer["Page Not Found (404)"],
                CanonicalUrl = null,
                CategoryPages = BaseViewModel.CategoryPages
            };
            HttpContext.Response.StatusCode = 404;
            return View($"{BaseViewModel.ViewPrefix}Error", model);
        }

        [HttpGet]
        public IActionResult Translate([FromQuery]TranslateRequestModel model)
        {
            var hits = service.Search(new SearchCriteria {
                Locale = model.SourceLocale,
                SearchType = SearchType.URN,
                BlogID = Blog.Id.Value,
                SearchTerm = model.Url                
            });

            if (hits.HitsCount > 0 && hits.Hits[0].Translations != null && hits.Hits[0].Translations.ContainsKey(model.TargetLocale))
                return LocalRedirect(hits.Hits[0].Translations[model.TargetLocale]);
            else if(urlMapper.HasTranslatedUrl(model.SourceLocale, model.TargetLocale, model.Url))
                return LocalRedirect(urlMapper.GetTranslatedUrl(model.SourceLocale, model.TargetLocale, model.Url));
            
            return LocalRedirect($"/{ model.TargetLocale.ToString().ToLowerInvariant() }");
        }

        public IActionResult SearchCategories(CategorySearchParameters param)
        {
            Hit category = null;
            var model = GetViewModel(new SearchParameters { Category = param.Category }, SearchSortOrder.BestMatch);
            model.CanonicalUrl = $"{model.BlogUrl.TrimEnd('/')}/{param.Locale.ToString().ToLowerInvariant()}/{param.Region.ToLowerInvariant()}/{param.Category}".TrimEnd('/');
            model.HrefLangUrls.Add(Blog.Locale.ToString().ToLowerInvariant(), model.CanonicalUrl);

            var categoryResults = service.Search(new SearchCriteria() { BlogID = Blog.Id.Value, Locale = Blog.Locale.Value, SearchTerm = Request.Path, SearchType = SearchType.URN });
            if (categoryResults.HitsCount > 0 && categoryResults.Hits[0] != null) {
                category = categoryResults.Hits[0];
                foreach (var translation in category.Translations)
                    model.HrefLangUrls.Add(translation.Key.ToString().ToLowerInvariant(), $"{Blog.Url}{translation.Value}");
            }

            model.Title = category == null ? param.Category : category.Title;
            model.Referrer = GetReferrer();
            UpdateBreadcrumbs(model, new List<Breadcrumb>{
                new Breadcrumb { Name = localizer["Home"], Url = localizer["/"] },
                new Breadcrumb { IsActive = true, Name = (category == null ? param.Category : category.Title) }
            });            
            return View($"{BaseViewModel.ViewPrefix}Search", model);
        }
    }
}