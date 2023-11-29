using System.Collections.Generic;
using System.Security.Claims;
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

        public IActionResult Login()
        {
            return View(new LoginViewModel());
        }

        [HttpPost]
        public IActionResult Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var configSection = _configuration.GetSection("Credentials");
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
                new(ClaimsIdentity.DefaultNameClaimType, model.UserName)
            };
                
            var id = new ClaimsIdentity(
                claims,
                "AvtoobvesCookie",
                ClaimsIdentity.DefaultNameClaimType,
                ClaimsIdentity.DefaultRoleClaimType);

            HttpContext
                .SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id))
                .GetAwaiter()
                .GetResult();
                
            return Redirect(returnUrl ?? Url.Action("Index", "Admin"));
        }
    }
}