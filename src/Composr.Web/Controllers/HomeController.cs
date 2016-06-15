using Composr.Core;
using Composr.Web.ViewModels;
using Microsoft.AspNet.Mvc;
using System.Linq;

namespace Composr.Web.Controllers
{
    public class HomeController : FrontEndController
    {
        private ISearchService service;

        public HomeController(ISearchService service, Blog blog):base(blog)
        {
            this.service = service;
        }

        // GET: /<controller>/
        public IActionResult Index()
        {
            var results = service.Search(new SearchCriteria() { BlogID = Blog.Id.Value, Locale = Blog.Locale.Value, SearchSortOrder = SearchSortOrder.DatePublished });
            var model = PostSearchViewModel.FromBaseFrontEndViewModel(BaseViewModel);
            model.SearchResults = results;
            model.Title = "Cocozil Home - Discover Mauritius";
            return View(model);
        }

        public IActionResult PostDetails(string postkey)
        {
            var results = service.Search(new SearchCriteria() { BlogID = Blog.Id.Value, Locale = Blog.Locale.Value, URN = HttpContext.Request.Path.Value});
            var model = PostSearchViewModel.FromBaseFrontEndViewModel(BaseViewModel);
            if (results != null && results.Count > 0)
            {
                model.Title = $"{results[0].Title} Recipe - Cocozil";
                model.MetaDescription = $"{results[0].MetaDescription}";
            }
            model.SearchResults = results;
            return View(model);
        }

        [HttpGet("api/autocomplete")]
        public IActionResult Search(string q)
        {
            var results = service.Search(new SearchCriteria() { BlogID = Blog.Id.Value, Locale = Blog.Locale.Value, SearchSortOrder = SearchSortOrder.BestMatch, Limit = 5, SearchTerm = q, SearchType = SearchType.AutoComplete });
            return new JsonResult(new { suggestions = results.Select(r => new { value = r.Title, data = r.URN }) });
        }
    }
}
