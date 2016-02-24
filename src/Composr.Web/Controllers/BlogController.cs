using Microsoft.AspNet.Mvc;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace Composr.Web.Controllers
{
    [Route("admin/blogs")]
    public class BlogController : Controller
    {
        public BlogController()
        {

        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
    }
}
