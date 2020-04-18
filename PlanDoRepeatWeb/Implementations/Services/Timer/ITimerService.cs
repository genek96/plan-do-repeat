using System.Collections.Generic;
using System.Threading.Tasks;
using PlanDoRepeatWeb.Models.Web;

namespace PlanDoRepeatWeb.Implementations.Services.Timer
{
    public interface ITimerService
    {
        Task<List<Models.Database.Timer>> GetAllTimersForUserAsync(string userId);

        Task CreateTimerAsync(string userId, TimerModel timer);

        Task UpdateTimerAsync(
            string timerId,
            string newName = null,
            string newDescription = null,
            int? newPeriod = null);

        Task DoActionOnTimer(string userId, string timerId, TimerAction action);
    }
}