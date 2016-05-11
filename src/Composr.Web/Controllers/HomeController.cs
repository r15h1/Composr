using Composr.Core;
using Composr.Core.Services;
using Microsoft.AspNet.Mvc;

namespace Composr.Web.Controllers
{
    [Route("")]
    public class HomeController : FrontEndController
    {
        private IRepoService<Post> service;

        public HomeController(IRepoService<Post> service, Blog blog):base(blog)
        {
            this.service = service;
        }

        // GET: /<controller>/
        public IActionResult Index()
        {
            return View(BaseViewModel);
        }
    }
}
