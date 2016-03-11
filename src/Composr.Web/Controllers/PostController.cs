using Composr.Core;
using Composr.Core.Repositories;
using Composr.Core.Services;
using Composr.Web.Models;
using Composr.Web.ViewModels;
using Microsoft.AspNet.Mvc;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace Composr.Web.Controllers
{
    [Route("admin/blogs/{blogid:int}/posts")]
    public class PostController : Controller
    {
        private IService<Post> service;
        private Blog blog;
        
        public PostController(IService<Post> service, Blog blog)
        {
            this.service = service;
            this.blog = blog;
        }

        [HttpGet("")]
        public IActionResult Index([FromQuery]PostRequestModel model)
        {
            Filter filter = new Filter() { Criteria= model.Search };
            return View(service.Get(filter));
        }

        [HttpGet("{postid:int}")] 
        public IActionResult Details(int postid)
        {
            Post post = service.Get(postid);
            PostViewModel model = new PostViewModel() { BlogId = post.Blog.Id, Id = post.Id, Body = post.Body, Title = post.Title , PostStatus = post.Status};
            return View(model);
        }

        [HttpGet("new")]
        public IActionResult Create()
        {
            return View("Details", new PostViewModel() { BlogId = blog.Id });
        }

        [HttpPost("new")]
        public IActionResult Create([FromForm]PostViewModel model)
        {
            return Save(model);
        }

        [HttpPost("{postid:int}")]
        public IActionResult Update([FromRoute]int postid, [FromForm]PostViewModel model)
        {
            return Save(model, postid);
        }

        private IActionResult Save(PostViewModel model, int? postid = null)
        {
            model.BlogId = blog.Id;
            if (!ModelState.IsValid)
                return View("Details", model);

            model.Id = postid;
            service.Save(MapPost(model));
            return RedirectToAction("Index");
        }

        private Post MapPost(PostViewModel model)
        {
            return new Post(blog)
            {
                Body = model.Body,
                Title = model.Title,
                Id = model.Id,
                Status = model.PostStatus
            };
        }
    }
}