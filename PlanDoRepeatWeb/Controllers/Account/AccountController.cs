using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using PlanDoRepeatWeb.Models.Authentication;

namespace PlanDoRepeatWeb.Controllers.Authentication
{
    public class AccountController : Controller
    {
        private readonly UsersService usersService;

        public AccountController(UsersService usersService)
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
                if (await usersService.RegisterAsync(registerModel.Email, registerModel.Password).ConfigureAwait(false))
                {
                    await AuthenticateAsync(registerModel.Email).ConfigureAwait(false);
                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError("", "Пользователь с такой почтой уже существует!");
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

        
        private Task AuthenticateAsync(string userName)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, userName)
            };

            ClaimsIdentity id = new ClaimsIdentity(
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
