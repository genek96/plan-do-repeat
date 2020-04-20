using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using PlanDoRepeatWeb.Implementations.Services;
using PlanDoRepeatWeb.Implementations.Services.Timer;
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
    }
}
