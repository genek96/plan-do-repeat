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
        public async Task<IActionResult> AllTimers()
        {
            var userId =  HttpContext.User.Identity.Name;
            var timers = await timerService
                .GetAllTimersForUserAsync(userId)
                .ConfigureAwait(false);
            return View(timers);
        }

        [Authorize]
        [HttpGet]
        public IActionResult AddNewTimer()
        {
            return RedirectToAction("AllTimers");
        }
        
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddNewTimer(TimerModel newTimer)
        {
            await timerService.CreateTimerAsync(HttpContext.User.Identity.Name, newTimer);
            return RedirectToAction("AllTimers");
        }
    }
}
