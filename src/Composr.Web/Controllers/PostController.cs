using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.Mvc;
using Composr.Core.Services;
using Composr.Core;
using Composr.Web.Models;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace Composr.Web.Controllers
{
    [Route("admin/blogs/{blogid:int}")]
    public class PostController : Controller
    {
        private IService<Post> service;

        public PostController(IService<Post> service)
        {
            this.service = service;
        }

        [HttpGet]
        public IActionResult Index(PostRequestModel model)
        {
            return View(model.BlogID);
        }
    }
}
