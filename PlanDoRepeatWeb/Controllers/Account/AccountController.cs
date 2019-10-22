using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using PlanDoRepeatWeb.Models.Authentication;
using PlanDoRepeatWeb.Models;
using System.Security.Cryptography;
using Commons.StringHelpers;

namespace PlanDoRepeatWeb.Controllers.Authentication
{
    public class AccountController : Controller
    {
        private readonly UserRepository userRepository;

        public AccountController(UserRepository userService)
        {
            this.userRepository = userService;
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
                var user = await userRepository
                    .GetUserByLoginAsync(loginModel.Email)
                    .ConfigureAwait(false);

                if (user != null && user.Password == HashPassword(loginModel.Password))
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
                var user = await userRepository
                    .GetUserByLoginAsync(registerModel.Email)
                    .ConfigureAwait(false);
                
                if (user == null)
                {
                    await userRepository
                        .CreateUserAsync(
                            new User
                            {
                                Login = registerModel.Email,
                                Password = HashPassword(registerModel.Password)
                            })
                        .ConfigureAwait(false);

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

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                return sha256.ComputeHash(password.ToByteArray()).ToUtf8String();
            }
        }
    }
}
