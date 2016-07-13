using Composr.Core;
using Microsoft.AspNetCore.Http;
using SaasKit.Multitenancy;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Composr.Web.MultiTenancy
{
    public class BlogResolver : ITenantResolver<Blog>
    {
        private static IEnumerable<Blog> blogs = new List<Blog>();

        public BlogResolver(IRepository<Blog> repository)
        {
            if (blogs == null || !blogs.Any())
                blogs = repository.Get(null);
        }        

        public Task<TenantContext<Blog>> ResolveAsync(HttpContext context)
        {
            TenantContext<Blog> ctx = null;
            Blog blog = null;

            if (context.Request.Path.HasValue && context.Request.Path.Value.StartsWith("/admin/blogs"))
                blog = blogs.FirstOrDefault(t => context.Request.Path.Value.StartsWith("/admin/blogs/" + t.Id.ToString()));
            else
                blog = blogs.FirstOrDefault(t => t.Url.Replace("www.","").Replace("http://","").Equals(context.Request.Host.Value.ToLower().Replace("www.", "")));

            if (blog != null) ctx = new TenantContext<Blog>(blog);        

            return Task.FromResult(ctx);
        }
    }
}