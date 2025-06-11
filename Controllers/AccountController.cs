using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Avtoobves.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Avtoobves.Controllers
{
    public class AccountController : Controller
    {
        private readonly IConfiguration _configuration;

        public AccountController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult Login() => View(new LoginViewModel());

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var configSection = _configuration.GetSection("AdminCredentials");
            var isUsernameMatching = model.UserName == configSection.GetValue<string>("Username");
            var isPasswordMatching = model.Password == configSection.GetValue<string>("Password");

            if (!isUsernameMatching || !isPasswordMatching)
            {
                ModelState.AddModelError("", "Неправильное имя пользователя или пароль");
                TempData["fail"] = "Ошибка аутентификации!";

                return View();
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, model.UserName)
            };

            var identity = new ClaimsIdentity(
                claims,
                "ApplicationCookie",
                ClaimsIdentity.DefaultNameClaimType,
                ClaimsIdentity.DefaultRoleClaimType);

            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return Redirect(returnUrl ?? Url.Action("Index", "Admin"));
        }
    }
}