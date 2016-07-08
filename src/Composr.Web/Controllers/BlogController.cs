using Composr.Core;
using Composr.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace Composr.Web.Controllers
{
    [Authorize]
    [Route("admin/blogs")]
    public class BlogController : Controller
    {
        public BlogController(IRepoService<Blog> service)
        {
            this.service = service;
        }

        [HttpGet("")]
        public IActionResult Index([FromQuery] TestModel model)
        {
            //IEnumerable<Blog> blogs = service.Get(null);
            IEnumerable<Blog> blogs = service.Get(new Core.Filter { Criteria = model.Search });
            return View(blogs);
        }

        //[HttpGet("new")]
        //public IActionResult Create()
        //{
        //    return View("Details", new PostViewModel() { BlogId = blog.Id });
        //}

        //[HttpPost("new")]
        //public IActionResult Create([FromForm]PostViewModel model)
        //{
        //    return Save(model);
        //}

        private IRepoService<Blog> service;
    }
}
