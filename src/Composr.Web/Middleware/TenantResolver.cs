using Composr.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using SaasKit.Multitenancy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Composr.Web.Middleware
{
    public class BlogResolver : ITenantResolver<Blog>
    {
        private readonly string blogListCacheKey = "BlogListCacheKey";
        private List<Blog> blogs;

        public BlogResolver(IRepository<Blog> repository, IOptions<RequestLocalizationOptions> localizationOptions, IMemoryCache cache)
        {            
            if (!cache.TryGetValue(blogListCacheKey, out blogs))
            {
                blogs = new List<Blog>();
                foreach (var option in localizationOptions.Value.SupportedUICultures)
                {
                    if (Enum.IsDefined(typeof(Locale), option.TwoLetterISOLanguageName.ToUpperInvariant()))
                    {
                        repository.Locale = (Locale)Enum.Parse(typeof(Locale), option.TwoLetterISOLanguageName.ToUpperInvariant(), true);
                        blogs.AddRange(repository.Get(null));                        
                    }
                }
                cache.Set(blogListCacheKey, blogs, new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromHours(5)));
            }
        }        

        public Task<TenantContext<Blog>> ResolveAsync(HttpContext context)
        {            
            TenantContext<Blog> ctx = null;
            Blog blog = null;
            var requestCulture = context.Features.Get<IRequestCultureFeature>();

            if (context.Request.Path.HasValue && context.Request.Path.Value.StartsWith("/admin/blogs"))
            {
                blog = blogs.FirstOrDefault(t => context.Request.Path.Value.StartsWith("/admin/blogs/" + t.Id.ToString() + "/" + t.Locale.ToString().ToLowerInvariant()));
            }
            else
            {
                string host = context.Request.Host.Value.ToLower().Replace("www.", "");
                blog = blogs.FirstOrDefault(t => t.Url.Replace("www.", "").Replace("http://", "").Equals(host) && t.Locale.ToString().ToLowerInvariant().Equals(requestCulture.RequestCulture.Culture.TwoLetterISOLanguageName.ToLowerInvariant()));

                if(blog == null) blog = blogs.FirstOrDefault(t => t.Url.Replace("www.", "").Replace("http://", "").Equals(host));
            }

            if (blog != null) ctx = new TenantContext<Blog>(blog);        

            return Task.FromResult(ctx);
        }
    }
}