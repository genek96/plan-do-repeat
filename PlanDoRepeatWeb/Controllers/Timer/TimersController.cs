using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using PlanDoRepeatWeb.Implementations.Services;
using PlanDoRepeatWeb.Models.Web;

namespace PlanDoRepeatWeb.Controllers.Timer
{
    public class TimersController : Controller
    {
        private readonly ITimerService timerService;

        public TimersController(ITimerService timerService)
        {
            this.timerService = timerService;
        }

        [Authorize]
        [HttpGet]
        public IActionResult AllTimers()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AllTimers(TimerModel newTimer)
        {
            await timerService.CreateTimerAsync(HttpContext.User.Identity.Name, newTimer);
            return RedirectToAction("AllTimers");
        }
    }
}
