using Composr.Core;
using Composr.Web.ViewModels;
using Microsoft.AspNet.Mvc;
using System.Linq;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace Composr.Web.Controllers
{
    public class FrontEndController : Controller
    {
        protected Blog blog { get; set; }

        protected BaseViewModel BaseViewModel { get; }

        public FrontEndController(Blog blog)
        {
            this.blog = blog;
            BaseViewModel = new BaseViewModel() {
                BlogUrl = blog.Url,
                LogoUrl = blog.Attributes.SingleOrDefault(x => x.Key == BlogAttributeKeys.LogoUrl).Value,
                Copyright = blog.Attributes.SingleOrDefault(x => x.Key == BlogAttributeKeys.Copyright).Value,
            };
        }
    }
}
