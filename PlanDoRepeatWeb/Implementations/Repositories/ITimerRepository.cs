using System.Collections.Generic;
using System.Threading.Tasks;
using PlanDoRepeatWeb.Models.Database;

namespace PlanDoRepeatWeb.Implementations.Repositories
{
    public interface ITimerRepository
    {
        Task<List<Timer>> GetAllTimersAsync(string userId);
        Task<Timer> GetTimerAsync(string timerId);

        Task UpdateTimerMetaAsync(
            string timerId,
            TimerState newState,
            int passedSeconds,
            long lastUpdate);

        Task CreateTimerAsync(Timer timer);

        Task UpdateTimerAsync(
            string timerId,
            string newName = null,
            string newDescription = null,
            int? newPeriod = null);

        Task DeleteTimerAsync(string timerId);
    }
}