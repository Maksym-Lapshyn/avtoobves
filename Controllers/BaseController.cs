using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Globalization;
using System.Linq;

namespace Avtoobves.Controllers
{
    public class BaseController : Controller
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            // Get culture from route data
            var culture = context.RouteData.Values["culture"]?.ToString() ?? "ru";
            
            // Map short culture code to full culture name
            var cultureInfo = culture switch
            {
                "uk" => new CultureInfo("uk-UA"),
                "ru" => new CultureInfo("ru-UA"),
                _ => new CultureInfo("ru-UA")
            };

            // Set culture for current thread
            CultureInfo.CurrentCulture = cultureInfo;
            CultureInfo.CurrentUICulture = cultureInfo;

            // Store culture in ViewBag for views
            ViewBag.Culture = culture;
            ViewBag.CurrentCulture = cultureInfo.Name;

            base.OnActionExecuting(context);
        }
    }
}
