using Composr.Core;
using Composr.Lib.Services;
using Composr.Lib.Specifications;
using Composr.Web.MultiTenancy;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Mvc.Razor;
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

            services.Configure<RazorViewEngineOptions>(options =>
            {
                options.ViewLocationExpanders.Add(new TenantViewLocationExpander());
            });

            services.AddScoped<ISearchService, SearchService>();

            services.AddScoped<IRepository<Blog>, Repository.Sql.BlogRepository>();
            services.AddScoped<IRepository<Post>, Composr.Repository.Sql.PostRepository>();

            services.AddScoped<IBlogRepository, Repository.Sql.BlogRepository>();
            services.AddScoped<IPostRepository, Repository.Sql.PostRepository>();
            
            services.AddScoped<ISpecification<Blog>, MinimalBlogSpecification>();
            services.AddScoped<ISpecification<Post>, PostSpecification>();

            services.AddScoped<IRepoService<Post>, RepoService<Post>>();
            services.AddScoped<IRepoService<Blog>, RepoService<Blog>>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseStaticFiles();
            app.UseDeveloperExceptionPage();
            app.UseMultitenancy<Blog>();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "blog-empty-route",
                    template: "",
                    defaults: new { controller = "Home", action = "Index" }
                );

                routes.MapRoute(
                    name: "blog-default-route",
                    template: "blog",
                    defaults: new { controller = "Home", action = "Index" }
                );

                routes.MapRoute(
                    name: "blog-details-route",
                    template: "blog/{postkey}",
                    defaults: new { controller = "Home", action = "PostDetails" }
                );

                routes.MapRoute(
                    name: "recipe-details-route",
                    template: "mauritius/cooking/{postkey}",
                    defaults: new { controller = "Home", action = "PostDetails" }
                );

                routes.MapRoute(
                    name: "search-route",
                    template: "search",
                    defaults: new { controller = "Home", action = "Search" }
                );
            });            
        }

        // Entry point for the application.
        public static void Main(string[] args) => WebApplication.Run<Startup>(args);
    }
}
