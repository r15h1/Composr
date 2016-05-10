using Composr.Core;
using Composr.Web.ViewModels;
using Microsoft.AspNet.Mvc;
using System.Linq;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace Composr.Web.Controllers
{
    [Route("")]
    public class HomeController : FrontEndController
    {
        public HomeController(Blog blog):base(blog)
        {
            
        }

        // GET: /<controller>/
        public IActionResult Index()
        {
            return View(BaseViewModel);
        }
    }
}
