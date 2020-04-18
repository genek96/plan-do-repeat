using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PlanDoRepeatWeb.Implementations.Repositories;
using PlanDoRepeatWeb.Models.Database;
using PlanDoRepeatWeb.Models.Web;

namespace PlanDoRepeatWeb.Implementations.Services.Timer
{
    public class TimerService : ITimerService
    {
        private readonly ITimerRepository timerRepository;

        public TimerService(ITimerRepository timerRepository)
        {
            this.timerRepository = timerRepository;
        }

        public Task<List<Models.Database.Timer>> GetAllTimersForUserAsync(string userId) =>
            timerRepository.GetAllTimersAsync(userId);

        public Task CreateTimerAsync(string userId, TimerModel timer) =>
            timerRepository.CreateTimerAsync(new Models.Database.Timer
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

        public async Task DoActionOnTimer(string userId, string timerId, TimerAction action)
        {
            var currentTimer = await timerRepository.GetTimerAsync(timerId).ConfigureAwait(false);

            currentTimer = currentTimer ?? throw new ArgumentException($"Timer with {timerId} was not found!");
            ValidateUserHasAccessToTimer(userId, currentTimer);

            switch (action)
            {
                case TimerAction.Start:
                case TimerAction.Repeat:
                    await RunTimerAsync(currentTimer).ConfigureAwait(false);
                    break;
                case TimerAction.Stop:
                    await StopTimerAsync(currentTimer).ConfigureAwait(false);
                    break;
                case TimerAction.Pause:
                    await PauseTimerAsync(currentTimer).ConfigureAwait(false);
                    break;
                case TimerAction.Expire:
                    await ExpireTimerAsync(currentTimer).ConfigureAwait(false);
                    break;
                case TimerAction.Delete:
                    await DeleteTimerAsync(currentTimer).ConfigureAwait(false);
                    break;
                default:
                    throw new ArgumentException("Handler for the given action was not found");
            }
        }

        private Task StopTimerAsync(Models.Database.Timer currentTimer)
        {
            return currentTimer.State == TimerState.Stopped
                ? Task.CompletedTask
                : timerRepository.UpdateTimerStateAsync(
                    currentTimer.Id,
                    TimerState.Stopped,
                    0,
                    DateTime.UtcNow.Ticks);
        }

        private Task RunTimerAsync(Models.Database.Timer currentTimer)
        {
            var passedSeconds = 0;
            var currentTime = DateTime.UtcNow.Ticks;
            if (currentTimer.State == TimerState.Paused)
            {
                passedSeconds = currentTimer.PassedSeconds
                                + (int) TimeSpan.FromTicks(currentTime - currentTimer.LastUpdate).TotalSeconds;
            }

            return timerRepository.UpdateTimerStateAsync(currentTimer.Id, TimerState.Active, passedSeconds,
                currentTime);
        }

        private Task DeleteTimerAsync(Models.Database.Timer currentTimer)
        {
            return timerRepository.DeleteTimerAsync(currentTimer.Id);
        }

        private Task PauseTimerAsync(Models.Database.Timer currentTimer)
        {
            var currentTime = DateTime.UtcNow.Ticks;
            var timePassed = currentTimer.PassedSeconds
                             + (int) TimeSpan.FromTicks(currentTime - currentTimer.LastUpdate).TotalSeconds;

            if (timePassed > currentTimer.PeriodInSeconds)
            {
                timePassed = 0;
            }

            return timerRepository.UpdateTimerStateAsync(currentTimer.Id, TimerState.Paused, timePassed, currentTime);
        }

        private Task ExpireTimerAsync(Models.Database.Timer currentTimer)
        {
            if (currentTimer.State != TimerState.Active)
            {
                return Task.CompletedTask;
            }

            return timerRepository.UpdateTimerStateAsync(
                currentTimer.Id,
                TimerState.Expired,
                currentTimer.PeriodInSeconds,
                DateTime.UtcNow.Ticks);
        }

        private static void ValidateUserHasAccessToTimer(string userId, Models.Database.Timer timer)
        {
            if (timer.UserId != userId)
            {
                throw new UnauthorizedAccessException(
                    $"The timer with id={timer.Id} doesn't belong to authenticated user!");
            }
        }
    }
}