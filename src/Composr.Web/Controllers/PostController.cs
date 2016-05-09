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
            //ViewData["logo"] = blog.Logo;
            return View(service.Get(filter));
        }

        [HttpGet("{postid:int}")] 
        public IActionResult Details(int postid)
        {
            Post post = service.Get(postid);
            PostViewModel viewModel = new PostViewModel() { BlogId = post.Blog.Id, Id = post.Id, Body = post.Body, Title = post.Title , PostStatus = post.Status, URN = post.URN};
            if (post.Attributes.ContainsKey(PostAttributeKeys.MetaDescription))
                viewModel.MetaDescription = post.Attributes[PostAttributeKeys.MetaDescription];

            //ViewData["logo"] = blog.Logo;
            return View(viewModel);
        }

        [HttpGet("new")]
        public IActionResult Create()
        {
            //ViewData["logo"] = blog.Logo;
            return View("Details", new PostViewModel() { BlogId = blog.Id });
        }

        [HttpPost("new")]
        public IActionResult Create([FromForm]PostViewModel model)
        {
            //ViewData["logo"] = blog.Logo;
            return Save(model);
        }

        [HttpPost("{postid:int}")]
        public IActionResult Update([FromRoute]int postid, [FromForm]PostViewModel viewModel)
        {
            return Save(viewModel, postid);
        }

        private IActionResult Save(PostViewModel viewModel, int? postid = null)
        {
            viewModel.BlogId = blog.Id;
            if (!ModelState.IsValid)
                return View("Details", viewModel);

            viewModel.Id = postid;
            service.Save(MapPost(viewModel));
            return RedirectToAction("Index");
        }

        private Post MapPost(PostViewModel viewModel)
        {
            var post = new Post(blog)
            {
                Body = viewModel.Body,
                Title = viewModel.Title,
                Id = viewModel.Id,
                Status = viewModel.PostStatus,
                URN = viewModel.URN
            };
            post.Attributes.Add(PostAttributeKeys.MetaDescription, viewModel.MetaDescription);
            return post;
        }
    }
}