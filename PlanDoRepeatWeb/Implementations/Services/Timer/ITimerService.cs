using System.Collections.Generic;
using System.Threading.Tasks;
using PlanDoRepeatWeb.Models.Web;

namespace PlanDoRepeatWeb.Implementations.Services.Timer
{
    public interface ITimerService
    {
        Task<List<TimerModel>> GetAllTimersForUserAsync(string userId);

        Task CreateTimerAsync(string userId, NewTimerModel newTimer);

        Task UpdateTimerAsync(
            string timerId,
            string newName = null,
            string newDescription = null,
            int? newPeriod = null);

        Task DoActionOnTimer(string userId, string timerId, TimerAction action);
    }
}