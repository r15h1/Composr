using Composr.Core;
using Composr.Web.MultiTenancy;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Composr.Web
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMultitenancy<Blog, BlogResolver>();
            services.AddMvc();

            services.AddScoped<Composr.Core.Repositories.IRepository<Composr.Core.Blog>, Composr.Mock.Repositories.BlogRepository>();
            services.AddScoped<Composr.Core.Repositories.IRepository<Composr.Core.Post>, Composr.Mock.Repositories.PostRepository>();
            
            services.AddScoped<Composr.Core.Specifications.ISpecification<Composr.Core.Blog>, Composr.Specifications.MinimalBlogSpecification>();
            services.AddScoped<Composr.Core.Specifications.ISpecification<Composr.Core.Post>, Composr.Specifications.MinimalPostSpecification>();

            services.AddScoped<Composr.Core.Services.IService<Composr.Core.Post>, Composr.Services.Service<Composr.Core.Post>>();
            services.AddScoped<Composr.Core.Services.IService<Composr.Core.Blog>, Composr.Services.Service<Composr.Core.Blog>>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseStaticFiles();
            app.UseDeveloperExceptionPage();
            app.UseMultitenancy<Blog>();
            app.UseMvc();           
        }

        // Entry point for the application.
        public static void Main(string[] args) => WebApplication.Run<Startup>(args);
    }
}
