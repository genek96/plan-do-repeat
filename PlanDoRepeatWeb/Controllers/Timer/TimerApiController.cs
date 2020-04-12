using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlanDoRepeatWeb.Implementations.Services;
using System.Text.Json;
using Commons.StringHelpers;
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

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> StartTimer(string timerId)
        {
            if (timerId.IsNullOrEmpty())
            {
                return BadRequest($"{nameof(timerId)} must be specified!");
            }
            var userId =  HttpContext.User.Identity.Name;
            await timerService.RunTimerAsync(userId, timerId).ConfigureAwait(false);
            return NoContent();
        }
        
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> StopTimer(string timerId)
        {
            if (timerId.IsNullOrEmpty())
            {
                return BadRequest($"{nameof(timerId)} must be specified!");
            }
            var userId =  HttpContext.User.Identity.Name;
            await timerService.StopTimerAsync(userId, timerId).ConfigureAwait(false);
            return NoContent();
        }
        
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> PauseTimer(string timerId)
        {
            if (timerId.IsNullOrEmpty())
            {
                return BadRequest($"{nameof(timerId)} must be specified!");
            }
            var userId =  HttpContext.User.Identity.Name;
            await timerService.PauseTimerAsync(userId, timerId).ConfigureAwait(false);
            return NoContent();
        }
        
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> DeleteTimer(string timerId)
        {
            if (timerId.IsNullOrEmpty())
            {
                return BadRequest($"{nameof(timerId)} must be specified!");
            }
            var userId =  HttpContext.User.Identity.Name;
            await timerService.DeleteTimerAsync(userId, timerId).ConfigureAwait(false);
            return NoContent();
        }
    }
}