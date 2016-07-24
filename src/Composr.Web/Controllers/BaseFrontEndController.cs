﻿using Composr.Core;
using Composr.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace Composr.Web.Controllers
{
    public class BaseFrontEndController : Controller
    {
        protected Blog Blog { get; set; }

        protected BaseFrontEndViewModel BaseViewModel {
            get;
            set;
        }

        public BaseFrontEndController(Blog blog)
        {
            this.Blog = blog;
            BaseViewModel = new BaseFrontEndViewModel()
            {
                BlogUrl = blog.Url,
                LogoUrl = blog.Attributes.SingleOrDefault(x => x.Key == BlogAttributeKeys.LogoUrl).Value,
                Copyright = blog.Attributes.SingleOrDefault(x => x.Key == BlogAttributeKeys.Copyright).Value,
                Tagline = blog.Attributes.SingleOrDefault(x => x.Key == BlogAttributeKeys.Tagline).Value,
                ImageLocation = blog.Attributes.SingleOrDefault(x => x.Key == BlogAttributeKeys.ImageLocation).Value,
                AnalyticsTrackingJSCode = blog.Attributes.SingleOrDefault(x => x.Key == BlogAttributeKeys.AnalyticsTrackingJSCode).Value,
                AdPublisherJSCode = blog.Attributes.SingleOrDefault(x => x.Key == BlogAttributeKeys.AdPublisherJSCode).Value,
                CanonicalUrl = blog.Url
            };
        }
    }
}