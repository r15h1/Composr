using Composr.Core;
using Composr.Core.Services;
using Microsoft.AspNet.Http;
using SaasKit.Multitenancy;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Composr.Web.MultiTenancy
{
    public class BlogResolver : ITenantResolver<Blog>
    {
        private static IEnumerable<Blog> blogs = new List<Blog>();

        public BlogResolver(IRepoService<Blog> service)
        {
            if (blogs == null || !blogs.Any())
                blogs = service.Get(null);
        }        

        public Task<TenantContext<Blog>> ResolveAsync(HttpContext context)
        {
            TenantContext<Blog> ctx = null;
            Blog blog = null;

            if (context.Request.Path.HasValue && context.Request.Path.Value.StartsWith("/admin/blogs"))
                blog = blogs.FirstOrDefault(t => context.Request.Path.Value.StartsWith("/admin/blogs/" + t.Id.ToString()));
            else
                blog = blogs.FirstOrDefault(t => t.Url.Equals(context.Request.Host.Value.ToLower()));

            if (blog != null) ctx = new TenantContext<Blog>(blog);        

            return Task.FromResult(ctx);
        }
    }
}