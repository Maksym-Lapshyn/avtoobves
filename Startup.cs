using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Avtoobves.Infrastructure;
using System.Globalization;
using Avtoobves.Middleware;

namespace Avtoobves
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        private IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            var connection = Configuration.GetConnectionString("AvtoobvesDatabase");

            services.AddDbContext<Context>(options => options.UseSqlServer(connection));
            
            // Add localization services
            services.AddLocalization(options => options.ResourcesPath = "Resources");
            
            services.AddControllersWithViews()
                .AddViewLocalization()
                .AddDataAnnotationsLocalization();

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
            
            // Configure supported cultures
            var supportedCultures = new[]
            {
                new CultureInfo("ru"), // Russian as default
                new CultureInfo("uk")  // Ukrainian
            };

            // Configure request localization (mainly for validation)
            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture("ru"),
                SupportedCultures = supportedCultures,
                SupportedUICultures = supportedCultures
            });
            
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            
            // Add custom culture middleware after routing
            app.UseMiddleware<CultureMiddleware>();
            
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                // Culture-specific routes for both Russian and Ukrainian
                endpoints.MapControllerRoute(
                    name: "localized",
                    pattern: "{culture:regex(^(ru|uk)$)}/{controller=Home}/{action=Index}/{id?}");
                
                // Default route redirects to Russian
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}",
                    defaults: new { culture = "ru" });
            });
        }
    }
}