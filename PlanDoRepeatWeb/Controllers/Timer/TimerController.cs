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
using Microsoft.AspNetCore.Authorization;

namespace PlanDoRepeatWeb.Controllers.Timer
{
    public class TimersController : Controller
    {
        [Authorize]
        [HttpGet]
        public IActionResult AllTimers()
        {
            return View();
        }
    }
}
