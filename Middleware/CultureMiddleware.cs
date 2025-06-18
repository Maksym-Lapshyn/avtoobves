using Microsoft.AspNetCore.Http;
using System.Globalization;
using System.Threading.Tasks;

namespace Avtoobves.Middleware
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class CultureMiddleware
    {
        private readonly RequestDelegate _next;

        public CultureMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        // ReSharper disable once UnusedMember.Global
        public async Task InvokeAsync(HttpContext context)
        {
            var culture = context.Request.RouteValues["culture"]?.ToString();

            // If culture is explicitly set to "uk", use Ukrainian
            // Default to Russian for all other cases (including no culture prefix)
            var cultureInfo = culture == "uk"
                ? new CultureInfo("uk")
                : new CultureInfo("ru");
            
            CultureInfo.CurrentCulture = cultureInfo;
            CultureInfo.CurrentUICulture = cultureInfo;

            await _next(context);
        }
    }
} 