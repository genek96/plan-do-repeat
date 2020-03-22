using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using PlanDoRepeatWeb.Implementations.Services;
using PlanDoRepeatWeb.Models.Authentication;

namespace PlanDoRepeatWeb.Controllers.Account
{
    public class AccountController : Controller
    {
        private readonly IUsersService usersService;

        public AccountController(IUsersService usersService)
        {
            this.usersService = usersService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel loginModel)
        {
            if (ModelState.IsValid)
            {
                if (await usersService.LoginAsync(loginModel.Email, loginModel.Password).ConfigureAwait(false))
                {
                    await AuthenticateAsync(loginModel.Email).ConfigureAwait(false);
                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError("", "Некорректная почта или праоль");
            }

            return View(loginModel);
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterModel registerModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var user = await usersService
                        .RegisterAsync(registerModel)
                        .ConfigureAwait(false);

                    await AuthenticateAsync(user.Id).ConfigureAwait(false);
                    return RedirectToAction("AllTimers", "Timers");
                }
                catch (ArgumentException ex)
                {
                    ModelState.AddModelError(nameof(registerModel.Email), ex.Message);
                }
            }

            return View(registerModel);
        }

        public async Task<IActionResult> LogoutAsync()
        {
            await HttpContext
                .SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme)
                .ConfigureAwait(false);

            return RedirectToAction("Login", "Account");
        }


        private Task AuthenticateAsync(string userId)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, userId)
            };

            var id = new ClaimsIdentity(
                claims,
                "ApplicationCookie",
                ClaimsIdentity.DefaultNameClaimType,
                ClaimsIdentity.DefaultRoleClaimType);

            return HttpContext
                .SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(id));
        }
    }
}