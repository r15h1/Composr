using Composr.Core;
using Composr.Lib.Indexing;
using Composr.Lib.Specifications;
using Composr.Lib.Util;
using Composr.Web.Data;
using Composr.Web.Middleware;
using Composr.Web.Models;
using Composr.Web.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;

namespace Composr.Web
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("settings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"settings.{env.EnvironmentName}.json", optional: true)
                .AddJsonFile("redirections.json", optional: true, reloadOnChange: true);

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

            services.AddLocalization(options => options.ResourcesPath = "Resources");
            services.Configure<List<RedirectionMapping>>(Configuration.GetSection("Redirections"));

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddMultitenancy<Blog, BlogResolver>();
            services.AddMvc(
                options =>
                {
                    options.OutputFormatters.Add(new XmlSerializerOutputFormatter());
                })
                .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
                .AddDataAnnotationsLocalization();

            services.Configure<RequestLocalizationOptions>(options =>
            {                
                options.DefaultRequestCulture = new RequestCulture(culture: "en", uiCulture: "en");
                options.SupportedCultures = Settings.SupportedCultures;
                options.SupportedUICultures = Settings.SupportedCultures;
                options.RequestCultureProviders.Clear();
                options.RequestCultureProviders.Add(new RouteCultureProvider(options.DefaultRequestCulture));
            });

            services.AddMemoryCache();

            // Add application services.
            services.AddTransient<IEmailSender, AuthMessageSender>();
            services.AddTransient<ISmsSender, AuthMessageSender>();

            services.Configure<RazorViewEngineOptions>(options =>
            {
                options.ViewLocationExpanders.Add(new TenantViewLocationExpander());
            });

            services.AddScoped<IIndexWriter, IndexWriter>();
            services.AddScoped<IIndexGenerator, IndexGenerator>();
            services.AddScoped<ISearchService, SearchService>();

            services.AddScoped<IRepository<Blog>, Repository.Sql.BlogRepository>();
            services.AddScoped<IRepository<Post>, Composr.Repository.Sql.PostRepository>();

            services.AddScoped<IBlogRepository, Repository.Sql.BlogRepository>();
            services.AddScoped<IPostRepository, Repository.Sql.PostRepository>();

            services.AddScoped<ISpecification<Blog>, MinimalBlogSpecification>();
            services.AddScoped<ISpecification<Post>, PostSpecification>();

            services.AddSingleton<IUrlMapper, UrlMapper>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();            

            app.UseApplicationInsightsRequestTelemetry();


            app.Use((context, next) =>
            {
                var host = context.Request.Host;
                if (host.Host.ToLower().StartsWith("www."))
                {
                    context.Response.Redirect(context.Request.GetEncodedUrl().Replace("://www.", "://"),true);
                    return Task.FromResult(0);
                }
                return next();
            });

            var localizationOptions = new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture("en"),
                SupportedCultures = Settings.SupportedCultures,
                SupportedUICultures = Settings.SupportedCultures
            };
            localizationOptions.RequestCultureProviders.Clear();
            localizationOptions.RequestCultureProviders.Add(new RouteCultureProvider(localizationOptions.DefaultRequestCulture));
            app.UseRequestLocalization(localizationOptions);


            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
                //app.UseBrowserLink();
            }
            else
            {
                app.UseStatusCodePagesWithReExecute("/error/{0}");
            }

            app.UseApplicationInsightsExceptionTelemetry();
            app.UseStaticFiles();
            app.UseIdentity();
            
            app.UseMultitenancy<Blog>();
            //app.UseMiddleware<CompressionMiddleware>();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "search-route",
                    template: "en/search",
                    defaults: new { controller = "FrontEnd", action = "Search" }
                );

                routes.MapRoute(
                    name: "search-route-fr",
                    template: "fr/rechercher",
                    defaults: new { controller = "FrontEnd", action = "Search" }
                );

                routes.MapRoute(
                    name: "home",
                    template: "{culture:regex(^fr$)}",
                    defaults: new { controller = "FrontEnd", action = "Index" }
                );

                routes.MapRoute(
                    name: "error-route",
                    template: "{culture=en}/error/{error}",
                    defaults: new { controller = "FrontEnd", action = "Error" }
                );

                routes.MapRoute(
                    name: "blog-default-route",
                    template: "blog",
                    defaults: new { controller = "FrontEnd", action = "Index" }
                );

                routes.MapRoute(
                    name: "blog-details-route",
                    template: "blog/{postkey}",
                    defaults: new { controller = "FrontEnd", action = "PostDetails" }
                );

                routes.MapRoute(
                   name: "default",
                   template: "{controller=FrontEnd}/{action=Index}/{ id ?}"
               );

                routes.MapRoute(
                    name: "findpost",
                    template: "{*postkey}",
                    defaults: new { controller = "FrontEnd", action = "FindPost" }
               );
            });
        }
    }
}
