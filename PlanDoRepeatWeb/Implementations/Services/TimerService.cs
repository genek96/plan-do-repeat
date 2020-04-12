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
        private readonly ITimerRepository timerRepository;

        public TimerService(ITimerRepository timerRepository)
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

        public async Task StopTimerAsync(string userId, string timerId)
        {
            var currentTimer = await timerRepository
                .GetTimerAsync(timerId)
                .ConfigureAwait(false);
            currentTimer = currentTimer ?? throw new ArgumentException($"Timer with {timerId} was not found!");
            ValidateUserHasAccessToTimer(userId, currentTimer);

            if (currentTimer.State == TimerState.Stopped)
            {
                return;
            }

            await timerRepository
                .UpdateTimerMetaAsync(timerId, TimerState.Stopped, 0, DateTime.UtcNow.Ticks)
                .ConfigureAwait(false);
        }

        public async Task RunTimerAsync(string userId, string timerId)
        {
            var currentTimer = await timerRepository
                .GetTimerAsync(timerId)
                .ConfigureAwait(false);

            currentTimer = currentTimer ?? throw new ArgumentException($"Timer with {timerId} was not found!");
            ValidateUserHasAccessToTimer(userId, currentTimer);

            var passedSeconds = 0;
            var currentTime = DateTime.UtcNow.Ticks;
            if (currentTimer.State == TimerState.Paused)
            {
                passedSeconds = currentTimer.PassedSeconds
                                + (int) TimeSpan.FromTicks(currentTime - currentTimer.LastUpdate).TotalSeconds;
            }

            await timerRepository
                .UpdateTimerMetaAsync(timerId, TimerState.Active, passedSeconds, currentTime)
                .ConfigureAwait(false);
        }

        public async Task DeleteTimerAsync(string userId, string timerId)
        {
            var currentTimer = await timerRepository
                .GetTimerAsync(timerId)
                .ConfigureAwait(false);

            currentTimer = currentTimer ?? throw new ArgumentException($"Timer with {timerId} was not found!");
            ValidateUserHasAccessToTimer(userId, currentTimer);

            await timerRepository.DeleteTimerAsync(timerId).ConfigureAwait(false);
        }

        public async Task PauseTimerAsync(string userId, string timerId)
        {
            var currentTimer = await timerRepository
                .GetTimerAsync(timerId)
                .ConfigureAwait(false);

            currentTimer = currentTimer ?? throw new ArgumentException($"Timer with {timerId} was not found!");
            ValidateUserHasAccessToTimer(userId, currentTimer);

            var currentTime = DateTime.UtcNow.Ticks;
            var timePassed = currentTimer.PassedSeconds
                             + (int) TimeSpan.FromTicks(currentTime - currentTimer.LastUpdate).TotalSeconds;

            if (timePassed > currentTimer.PeriodInSeconds)
            {
                timePassed = 0;
            }

            await timerRepository
                .UpdateTimerMetaAsync(timerId, TimerState.Paused, timePassed, currentTime)
                .ConfigureAwait(false);
        }

        private void ValidateUserHasAccessToTimer(string userId, Timer timer)
        {
            if (timer.UserId != userId)
            {
                throw new UnauthorizedAccessException(
                    $"The timer with id={timer.Id} doesn't belong to authenticated user!");
            }
        }
    }
}