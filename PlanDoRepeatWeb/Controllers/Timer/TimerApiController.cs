using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlanDoRepeatWeb.Implementations.Services;
using System.Text.Json;
using PlanDoRepeatWeb.Models.Web;

namespace PlanDoRepeatWeb.Controllers.Timer
{
    public class TimerApiController : Controller
    {
        private readonly ITimerService timerService;

        public TimerApiController(ITimerService timerService)
        {
            this.timerService = timerService;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAllTimers()
        {
            var userId =  HttpContext.User.Identity.Name;
            var timers = await timerService
                .GetAllTimersForUserAsync(userId)
                .ConfigureAwait(false);
            return Content(JsonSerializer.Serialize(timers, timers.GetType()));
        }
    }
}