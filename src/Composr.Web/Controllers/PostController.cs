using Composr.Core;
using Composr.Lib.Indexing;
using Composr.Web.Models;
using Composr.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace Composr.Web.Controllers
{
    [Authorize]
    [Route("admin/blogs/{blogid:int}/{locale}/posts")]
    public class PostController : Controller
    {
        private Blog blog;
        private IPostRepository service;
        private IIndexGenerator indexGenerator;

        public PostController(Blog blog, IPostRepository service, IIndexGenerator indexGenerator)
        {
            this.blog = blog;
            this.service = service;
            this.indexGenerator = indexGenerator;
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
            PostViewModel viewModel = new PostViewModel() { BlogId = post.Blog.Id, Id = post.Id, Body = post.Body, Title = post.Title , PostStatus = post.Status, URN = post.URN, Language = blog.Locale.ToString()};

            if (post.Attributes.ContainsKey(PostAttributeKeys.MetaDescription)) viewModel.MetaDescription = post.Attributes[PostAttributeKeys.MetaDescription];
            if (post.Attributes.ContainsKey(PostAttributeKeys.Tags)) viewModel.Tags = post.Attributes[PostAttributeKeys.Tags];
            if (post.Images.Count > 0) {
                viewModel.ImageUrl = post.Images.FirstOrDefault().Url;
                viewModel.ImageCaption = post.Images.FirstOrDefault().Caption;
            }
            
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
            Task.Run(() => indexGenerator.BuildIndex(blog));
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

            if (!string.IsNullOrWhiteSpace(viewModel.ImageUrl) && !string.IsNullOrWhiteSpace(viewModel.ImageCaption))
            {
                post.Images.Add(new PostImage { Caption = viewModel.ImageCaption, Url = viewModel.ImageUrl, SequenceNumber = 1 });
            }

            if (!string.IsNullOrWhiteSpace(viewModel.MetaDescription)) post.Attributes.Add(PostAttributeKeys.MetaDescription, viewModel.MetaDescription);
            if (!string.IsNullOrWhiteSpace(viewModel.Tags)) post.Attributes.Add(PostAttributeKeys.Tags, viewModel.Tags);
            
            return post;
        }

        [HttpGet("{postid:int}/translations")]
        public IActionResult Translate([FromRoute]int postid)
        {
            return View("Translations", service.GetTranslatedPosts(postid));
        }

        [HttpPost("{postid:int}/translations")]
        public IActionResult Translate(string postid, string language)
        {
            int id;
            if (int.TryParse(postid, out id))
            {
                service.Translate(id, (Locale)Enum.Parse(typeof(Locale), language));
            }
            return View("Translations", service.GetTranslatedPosts(id));
        }
    }
}