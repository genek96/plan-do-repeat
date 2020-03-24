﻿using System.Collections.Generic;
using System.Threading.Tasks;
using PlanDoRepeatWeb.Models.Web;

namespace PlanDoRepeatWeb.Implementations.Services
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

        Task StopTimerAsync(string timerId);
        Task RunTimerAsync(string timerId);
        Task PauseTimerAsync(string timerId);
    }
}