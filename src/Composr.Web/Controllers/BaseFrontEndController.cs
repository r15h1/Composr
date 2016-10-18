using Composr.Core;
using Composr.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Composr.Web.Controllers
{
    public class BaseFrontEndController : Controller
    {
        private string blogCategoryPagesCacheKey;

        protected Blog Blog { get; set; }

        protected BaseFrontEndViewModel BaseViewModel {
            get;
            set;
        }

        public BaseFrontEndController(Blog blog, IBlogRepository blogRepository, IMemoryCache cache)
        {
            IList<Category> blogCategoryPages;
            this.Blog = blog;
            blogCategoryPagesCacheKey = $"BlogCategoryPagesCacheKey_{blog.Id}_{blog.Locale}";

            if(!cache.TryGetValue(blogCategoryPagesCacheKey, out blogCategoryPages))
            {
                blogCategoryPages = blogRepository.GetCategoryPages(blog);
                cache.Set(blogCategoryPagesCacheKey, blogCategoryPages, new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromHours(5)));
            }

            BaseViewModel = new BaseFrontEndViewModel()
            {
                BlogUrl = blog.Url,
                LogoUrl = blog.Attributes.SingleOrDefault(x => x.Key == BlogAttributeKeys.LogoUrl).Value,
                Copyright = blog.Attributes.SingleOrDefault(x => x.Key == BlogAttributeKeys.Copyright).Value,
                Tagline = blog.Attributes.SingleOrDefault(x => x.Key == BlogAttributeKeys.Tagline).Value,
                ImageLocation = blog.Attributes.SingleOrDefault(x => x.Key == BlogAttributeKeys.ImageLocation).Value,
                AnalyticsTrackingJSCode = blog.Attributes.SingleOrDefault(x => x.Key == BlogAttributeKeys.AnalyticsTrackingJSCode).Value,
                AdPublisherJSCode = blog.Attributes.SingleOrDefault(x => x.Key == BlogAttributeKeys.AdPublisherJSCode).Value,
                CanonicalUrl = blog.Url,
                ViewPrefix = blog.Attributes.SingleOrDefault(x => x.Key == BlogAttributeKeys.ViewPrefix).Value,
                CategoryPages = blogCategoryPages
            };
        }

        protected string GetReferrer()
        {
            if (Request != null && Request.Headers != null && Request.Headers.ContainsKey("Referer"))
                return Request.Headers["Referer"].ToString();

            return null;
        }
    }
}
