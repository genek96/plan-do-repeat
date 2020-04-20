using PlanDoRepeatWeb.Models.Database;
using PlanDoRepeatWeb.Models.Web;

namespace PlanDoRepeatWeb.Models
{
    internal static class TimerExtensions
    {
        public static TimerModel ToTimerModel(this Timer timer)
        {
            return new TimerModel
            {
                Id = timer.Id,
                Name = timer.Name,
                Description = timer.Description,
                ElapsedSeconds = timer.PassedSeconds,
                LastUpdate = timer.LastUpdate,
                PeriodInSeconds = timer.PeriodInSeconds,
                State = timer.State
            };
        }
    }
}