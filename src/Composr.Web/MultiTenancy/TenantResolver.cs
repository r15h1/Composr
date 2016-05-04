using Composr.Core;
using Microsoft.AspNet.Http;
using SaasKit.Multitenancy;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Composr.Web.MultiTenancy
{
    public class BlogResolver : ITenantResolver<Blog>
    {
        IEnumerable<Blog> blogs = new List<Blog>(new[]
        {
            new Core.Blog(1) {Description="Blog 1 description", Locale=Core.Locale.EN, Name="blog1", Url="localhost:5000", Logo="~/img/cocozil.png" },
            new Core.Blog(2) {Description="Blog 2 description", Locale=Core.Locale.EN, Name="Blog2", Url="localhost:5001" },
            new Core.Blog(3) {Description="Blog 3 description", Locale=Core.Locale.EN, Name="Blog3", Url="localhost:5002" },
            new Core.Blog(4) {Description="Blog 4 description", Locale=Core.Locale.EN, Name="Blog4", Url="localhost:5003" }

        });
        public Task<TenantContext<Blog>> ResolveAsync(HttpContext context)
        {
            TenantContext<Blog> ctx = null;
            Blog blog = null;

            if (context.Request.Path.HasValue && context.Request.Path.Value.StartsWith("/admin/blogs"))
            {
                blog = blogs.FirstOrDefault(t => context.Request.Path.Value.StartsWith("/admin/blogs/" + t.Id.ToString()));
            }
            else {
                blog = blogs.FirstOrDefault(t => t.Url.Equals(context.Request.Host.Value.ToLower()));
            }

            if (blog != null)
            {
                ctx = new TenantContext<Blog>(blog);
            }

            return Task.FromResult(ctx);
        }
    }
}
