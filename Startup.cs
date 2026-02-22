using Avtoobves.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Avtoobves.Infrastructure;
using Microsoft.AspNetCore.Localization;
using System.Collections.Generic;
using System.Globalization;

namespace Avtoobves
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            var connection = Configuration.GetConnectionString("AvtoobvesDatabase");

            services.AddDbContext<Context>(options => options.UseSqlServer(connection));
            services.AddControllersWithViews();

            // Localization configuration
            services.AddLocalization(options => options.ResourcesPath = "Resources");
            services.Configure<RequestLocalizationOptions>(options =>
            {
                var supportedCultures = new[]
                {
                    new CultureInfo("ru-UA"),
                    new CultureInfo("uk-UA")
                };
                
                options.DefaultRequestCulture = new RequestCulture("ru-UA");
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;
                
                options.RequestCultureProviders = new List<IRequestCultureProvider>
                {
                    new RouteDataRequestCultureProvider { Options = options }
                };
            });

            services
                .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options => { options.LoginPath = "/Account/Login"; });

            services.AddTransient<IProductRepository, ProductRepository>();
            services.AddTransient<IBlogPostRepository, BlogPostRepository>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
            
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseRequestLocalization();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                // Localized route with culture prefix
                endpoints.MapControllerRoute(
                    name: "localized",
                    pattern: "{culture:regex(^(ru|uk)$)}/{controller=Home}/{action=Index}/{id?}");
                
                // Default route redirects to Russian version
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}",
                    defaults: new { culture = "ru" });
            });
        }
    }
}