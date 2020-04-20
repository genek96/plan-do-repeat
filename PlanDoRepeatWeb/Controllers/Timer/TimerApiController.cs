using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Commons.StringHelpers;
using PlanDoRepeatWeb.Implementations.Services.Timer;
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
            var userId = HttpContext.User.Identity.Name;
            var timers = await timerService
                .GetAllTimersForUserAsync(userId)
                .ConfigureAwait(false);
            return Content(JsonSerializer.Serialize(timers, timers.GetType()));
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> DoActionOnTimer(string timerId, [FromQuery] TimerAction action)
        {
            if (timerId.IsNullOrEmpty())
            {
                return BadRequest($"{nameof(timerId)} must be specified!");
            }

            var userId = HttpContext.User.Identity.Name;
            await timerService.DoActionOnTimer(userId, timerId, action).ConfigureAwait(false);
            return NoContent();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateTimer([FromBody] NewTimerModel newNewTimer)
        {
            await timerService.CreateTimerAsync(HttpContext.User.Identity.Name, newNewTimer);
            return Created("/Timers/AllTimers", null);
        }
    }
}