using Composr.Core;
using Composr.Core.Repositories;
using Composr.Core.Services;
using Composr.Web.Models;
using Microsoft.AspNet.Mvc;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace Composr.Web.Controllers
{
    [Route("admin/blogs/{blogid:int}")]
    public class PostController : Controller
    {
        private IService<Post> service;
        private Blog blog;

        public PostController(IService<Post> service, Blog blog)
        {
            this.service = service;
            this.blog = blog;
        }

        [HttpGet]
        [Route("")]
        [Route("posts")]
        public IActionResult Index(PostRequestModel model)
        {
            Filter filter = new Filter() { Criteria= model.Search };
            return View(service.Get(filter));
        }

        [HttpGet]
        [Route("posts/{postid:int}")]
        public IActionResult Details(int postid)
        {
            return View(service.Get(postid));
        }

        [HttpGet]
        [Route("posts/new")]
        public IActionResult Create()
        {
            return View(blog);
        }
    }
}
