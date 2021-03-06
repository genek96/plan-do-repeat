﻿using System.Collections.Generic;
using System.Threading.Tasks;
using PlanDoRepeatWeb.Models;
using PlanDoRepeatWeb.Models.Database;

namespace PlanDoRepeatWeb.Implementations.Repositories
{
	public interface ITimerRepository
	{
		IAsyncEnumerable<Timer> GetAllTimersAsync(string userId);
		ValueTask<Timer> GetTimerAsync(string timerId);

		Task UpdateTimerStateAsync(
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