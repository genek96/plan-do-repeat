using MongoDB.Driver;
using PlanDoRepeatWeb.Configurations.DatabaseSettings;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Commons.MongoDB;
using PlanDoRepeatWeb.Models.Database;

namespace PlanDoRepeatWeb.Models.Timer
{
    public class TimerRepository : MongoDbContext<Database.Timer>
    {
        public TimerRepository(TimerDatabaseSettings settings) 
            : base(settings)
        {
        }

        public Task<List<Database.Timer>> GetAllTimersAsync(string userId) => Entities
                .Find(x => x.UserId == userId)
                .ToListAsync();

        public Task<Database.Timer> GetTimerAsync(string timerId) => Entities
            .Find(timer => timer.Id == timerId)
            .FirstOrDefaultAsync();

        public Task UpdateTimerMetaAsync(
            string timerId,
            TimerState newState,
            int passedSeconds,
            long lastUpdate)
        {
            return Entities.UpdateOneAsync(
               new FilterDefinitionBuilder<Database.Timer>().Where(timer => timer.Id == timerId),
               Builders<Database.Timer>.Update
                   .Set(timer => timer.State, newState)
                   .Set(timer => timer.PassedSeconds, passedSeconds)
                   .Set(timer => timer.LastUpdate, lastUpdate));
        }

        public Task CreateTimerAsync(Database.Timer timer)
        {
            ValidatePeriod(timer.PeriodInSeconds);
            return Entities
                .InsertOneAsync(
                    new Database.Timer
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
            var filter = new FilterDefinitionBuilder<Database.Timer>().Where(timer => timer.Id == timerId);
            var updateDefinition = Builders<Database.Timer>.Update
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
