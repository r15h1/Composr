using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Composr.Web.Data;
using Composr.Web.Models;
using Composr.Web.Services;
using Composr.Web.MultiTenancy;
using Composr.Core;
using Composr.Lib.Services;
using Microsoft.AspNetCore.Mvc.Razor;
using Composr.Lib.Specifications;
using Composr.Lib.Util;

namespace Composr.Web
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("settings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"settings.{env.EnvironmentName}.json", optional: true);

            if (env.IsDevelopment())
            {
                //For more details on using the user secret store see http://go.microsoft.com/fwlink/?LinkID=532709
                builder.AddUserSecrets();

                //This will push telemetry data through Application Insights pipeline faster, allowing you to view results immediately.
               builder.AddApplicationInsightsSettings(developerMode: true);
            }

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
            Settings.Config = Configuration;
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {            
            services.AddSingleton<IConfiguration>(Configuration);
            // Add framework services.
            services.AddApplicationInsightsTelemetry(Configuration);

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddMultitenancy<Blog, BlogResolver>();
            services.AddMvc();

            // Add application services.
            services.AddTransient<IEmailSender, AuthMessageSender>();
            services.AddTransient<ISmsSender, AuthMessageSender>();

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
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();            

            app.UseApplicationInsightsRequestTelemetry();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseStatusCodePagesWithRedirects("/error/{0}");
            }

            app.UseApplicationInsightsExceptionTelemetry();

            app.UseStaticFiles();
            app.UseIdentity();

            // Add external authentication middleware below. To configure them please see http://go.microsoft.com/fwlink/?LinkID=532715
            
            app.UseMultitenancy<Blog>();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "error-route",
                    template: "error/{error}",
                    defaults: new { controller = "Home", action = "Error" }
                );

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

                routes.MapRoute(
                   name: "account",
                   template: "{controller=Account}/{action=Login}/{ id ?}"
               );
            });
        }
    }
}
