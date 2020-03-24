using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PlanDoRepeatWeb.Implementations.Repositories;
using PlanDoRepeatWeb.Models.Database;
using PlanDoRepeatWeb.Models.Web;

namespace PlanDoRepeatWeb.Implementations.Services
{
    public class TimerService : ITimerService
    {
        private readonly TimerRepository timerRepository;

        public TimerService(TimerRepository timerRepository)
        {
            this.timerRepository = timerRepository;
        }

        public Task<List<Timer>> GetAllTimersForUserAsync(string userId) =>
            timerRepository.GetAllTimersAsync(userId);

        public Task CreateTimerAsync(string userId, TimerModel timer) =>
            timerRepository.CreateTimerAsync(new Timer
            {
                Name = timer.Name,
                Description = timer.Description,
                PeriodInSeconds = timer.Period,
                UserId = userId
            });

        public Task UpdateTimerAsync(
            string timerId,
            string newName = null,
            string newDescription = null,
            int? newPeriod = null) =>
            timerRepository.UpdateTimerAsync(timerId, newName, newDescription, newPeriod);

        public Task StopTimerAsync(string timerId)
        {
            return timerRepository.UpdateTimerMetaAsync(timerId, TimerState.Stopped, 0, DateTime.UtcNow.Ticks);
        }

        public async Task RunTimerAsync(string timerId)
        {
            var currentTimer = await timerRepository
                .GetTimerAsync(timerId)
                .ConfigureAwait(false);

            var passedSeconds = 0;
            var currentTime = DateTime.UtcNow.Ticks;
            if (currentTimer.State == TimerState.Paused)
            {
                passedSeconds = currentTimer.PassedSeconds
                    + (int)TimeSpan.FromTicks(currentTime - currentTimer.LastUpdate).TotalSeconds;
            }

            await timerRepository
                .UpdateTimerMetaAsync(timerId, TimerState.Active, passedSeconds, currentTime)
                .ConfigureAwait(false);
        }

        public async Task PauseTimerAsync(string timerId)
        {
            var currentTimer = await timerRepository
                .GetTimerAsync(timerId)
                .ConfigureAwait(false);

            var currentTime = DateTime.UtcNow.Ticks;
            var timePassed = currentTimer.PassedSeconds
                + (int)TimeSpan.FromTicks(currentTime - currentTimer.LastUpdate).TotalSeconds;

            if (timePassed > currentTimer.PeriodInSeconds)
            {
                timePassed = 0;
            }

            await timerRepository
                .UpdateTimerMetaAsync(timerId, TimerState.Paused, timePassed, currentTime)
                .ConfigureAwait(false);
        }
    }
}
