﻿using MongoDB.Driver;
using PlanDoRepeatWeb.Configurations.DatabaseSettings;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PlanDoRepeatWeb.Models.Timer
{
    public class TimerRepository : MongoDBContext<Timer>
    {
        public TimerRepository(TimerDatabaseSettings settings) 
            : base(settings)
        {
        }

        public Task<List<Timer>> GetAllTimersAsync(string userId) => Entities
                .Find(x => x.UserId == userId)
                .ToListAsync();

        public Task<Timer> GetTimerAsync(string timerId) => Entities
            .Find(timer => timer.Id == timerId)
            .FirstOrDefaultAsync();

        public Task StopTimerAsync(string timerId) => Entities
            .UpdateOneAsync(
            new FilterDefinitionBuilder<Timer>().Where(timer => timer.Id == timerId),
            Builders<Timer>.Update
                .Set(timer => timer.State, TimerState.Stoped)
                .Set(timer => timer.PassedSeconds, 0)
                .Set(timer => timer.LastUpdate, DateTime.UtcNow.Ticks));

        public async Task PauseTimerAsync(string timerId)
        {
            var currentTimer = await GetTimerAsync(timerId).ConfigureAwait(false);

            var currentTime = DateTime.UtcNow.Ticks;
            var timePassed = currentTimer.PassedSeconds
                + (int)TimeSpan.FromTicks(currentTime - currentTimer.LastUpdate).TotalSeconds;

            if (timePassed > currentTimer.PeriodInSeconds)
            {
                timePassed = 0;
            }

            await Entities
                .UpdateOneAsync(
                    new FilterDefinitionBuilder<Timer>().Where(timer => timer.Id == timerId),
                    Builders<Timer>.Update
                        .Set(timer => timer.State, TimerState.Paused)
                        .Set(timer => timer.PassedSeconds, timePassed)
                        .Set(timer => timer.LastUpdate, currentTime))
                .ConfigureAwait(false);
        }

        public async Task RunTimerAsync(string timerId)
        {
            var currentTimer = await GetTimerAsync(timerId).ConfigureAwait(false);

            var passedSeconds = 0;
            var currentTime = DateTime.UtcNow.Ticks;
            if (currentTimer.State == TimerState.Paused)
            {
                passedSeconds = currentTimer.PassedSeconds
                    + (int)TimeSpan.FromTicks(currentTime - currentTimer.LastUpdate).TotalSeconds;
            }

            await Entities
                .UpdateOneAsync(
                    new FilterDefinitionBuilder<Timer>().Where(timer => timer.Id == timerId),
                    Builders<Timer>.Update
                        .Set(timer => timer.State, TimerState.Active)
                        .Set(timer => timer.PassedSeconds, passedSeconds)
                        .Set(timer => timer.LastUpdate, currentTime))
                .ConfigureAwait(false);
        }

        public Task CreateTimerAsync(Timer timer)
        {
            ValidatePeriod(timer.PeriodInSeconds);
            return Entities
                .InsertOneAsync(
                    new Timer
                    {
                        Name = timer.Name,
                        Description = timer.Description,
                        PeriodInSeconds = timer.PeriodInSeconds,
                        UserId = timer.UserId,
                        State = TimerState.Active,
                        PassedSeconds = 0,
                        LastUpdate = DateTime.UtcNow.Ticks
                    });
        }

        public Task UpdateTimerAsync(
            string timerId,
            string newName = null,
            string newDescription = null,
            int? newPeriod = null)
        {
            var filter = new FilterDefinitionBuilder<Timer>().Where(timer => timer.Id == timerId);
            var updateDefinition = Builders<Timer>.Update
                .Set(timer => timer.PassedSeconds, 0)
                .Set(timer => timer.State, TimerState.Paused)
                .Set(timer => timer.LastUpdate, DateTime.UtcNow.Ticks);

            if (newName != null)
            {
                updateDefinition = updateDefinition.Set(timer => timer.Name, newName);
            }
            if (newDescription != null)
            {
                updateDefinition = updateDefinition.Set(timer => timer.Description, newDescription);
            }
            if (newPeriod.HasValue)
            {
                ValidatePeriod(newPeriod.Value);
                updateDefinition.Set(timer => timer.PeriodInSeconds, newPeriod.Value);
            }

            return Entities.UpdateOneAsync(filter, updateDefinition);
        }

        private static void ValidatePeriod(int periodInSeconds)
        {
            if (periodInSeconds < 30)
            {
                throw new ArgumentException("Time period can not be less than 30 seconds!");
            }
        }
    }
}
