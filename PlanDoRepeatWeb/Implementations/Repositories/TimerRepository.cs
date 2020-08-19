using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Commons.MongoDB;
using MongoDB.Driver;
using PlanDoRepeatWeb.Configurations.DatabaseSettings;
using PlanDoRepeatWeb.Models;
using PlanDoRepeatWeb.Models.Database;

namespace PlanDoRepeatWeb.Implementations.Repositories
{
	public class TimerRepository : MongoDbRepository<Timer>, ITimerRepository
	{
		public TimerRepository(TimerDatabaseSettings settings)
			: base(settings)
		{
		}

		public IAsyncEnumerable<Timer> GetAllTimersAsync(string userId)
		{
			return FindAsync(t => t.UserId == userId);
		}

		public ValueTask<Timer> GetTimerAsync(string timerId)
		{
			return FindAsync(t => t.Id == timerId).FirstOrDefaultAsync();
		}

		public Task UpdateTimerStateAsync(
			string timerId,
			TimerState newState,
			int passedSeconds,
			long lastUpdate)
		{
			return UpdateAsync(
				t => t.Id == timerId,
				b => b.Set(t => t.State, newState)
					.Set(t => t.PassedSeconds, passedSeconds)
					.Set(t => t.LastUpdate, lastUpdate));
		}

		public Task CreateTimerAsync(Timer timer)
		{
			ValidatePeriod(timer.PeriodInSeconds);
			return AddAsync(new Timer
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
			return UpdateAsync(
				timer => timer.Id == timerId,
				b =>
				{
					if (newName != null)
					{
						b.Set(t => t.Name, newName);
					}

					if (newDescription != null)
					{
						b.Set(t => t.Description, newDescription);
					}

					if (newPeriod.HasValue)
					{
						ValidatePeriod(newPeriod.Value);
						b.Set(t => t.PeriodInSeconds, newPeriod);
					}

					return b.Set(t => t.PassedSeconds, 0)
						.Set(t => t.State, TimerState.Paused)
						.Set(t => t.LastUpdate, DateTime.UtcNow.Ticks);
				});
		}

		public Task DeleteTimerAsync(string timerId)
		{
			return DeleteOneAsync(timer => timer.Id == timerId);
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