using Composr.Core;
using Composr.Core.Services;
using Composr.Web.ViewModels;
using Microsoft.AspNet.Mvc;

namespace Composr.Web.Controllers
{
    [Route("")]
    [Route("blog")]
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
            var results = service.Search(new SearchCriteria() { BlogID = blog.Id.Value, Locale = blog.Locale.Value, SearchSortOrder = SearchSortOrder.DatePublished });
            var model = PostSearchViewModel.FromBaseFrontEndViewModel(BaseViewModel);
            model.SearchResults = results;
            return View(model);
        }

        [Route("blog/{postkey}")]
        public IActionResult PostDetails(string postkey)
        {
            var results = service.Search(new SearchCriteria() { BlogID = blog.Id.Value, Locale = blog.Locale.Value, URN = $"/blog/{postkey}" });
            var model = PostSearchViewModel.FromBaseFrontEndViewModel(BaseViewModel);
            model.SearchResults = results;
            return View(model);
        }
    }
}
