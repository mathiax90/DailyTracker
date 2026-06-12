using System;
using System.Collections.Generic;
using System.Text;
using DailyTracker.Domain.Repositories;

namespace DailyTracker.Domain.Entities.Activities;

public class ActivityFactory : IActivityFactory
{
    private readonly IActivityRepository _activityRepository;

    public ActivityFactory(IActivityRepository activityRepository)
    {
        _activityRepository = activityRepository;
    }
    public async Task<DurationActivity> CreateDurationActivityAsync(Guid userId, ActivityType activityType, DateTime date, TimeSpan duration, CancellationToken cancellationToken = default)
    {
        Duration d = new Duration(duration);
        await CheckLimitsAsync(userId, activityType, date);
        return new DurationActivity(userId, activityType, date, d);
    }

    public async Task<TimeOfDayActivity> CreateTimeActivityAsync(Guid userId, ActivityType activityType, DateTime date, CancellationToken cancellationToken = default)
    {
        await CheckLimitsAsync(userId, activityType, date);
        return new TimeOfDayActivity(userId, activityType, date);
    }

    public async Task<WeightActivity> CreateWeightActivityAsync(Guid userId, ActivityType activityType, DateTime date, double weight, CancellationToken cancellationToken = default)
    {
        await CheckLimitsAsync(userId, activityType, date);
        return new WeightActivity(userId, activityType, date, new Weight(weight));
    }

    /// <summary>
    /// Проверяет достигнуты ли лимиты по типу события
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="activityType"></param>
    /// <param name="date"></param>
    /// <returns></returns>
    /// <exception cref="DomainException"></exception>
    private async Task CheckLimitsAsync(Guid userId, ActivityType activityType, DateTime date)
    {
        if (await _activityRepository.IsActivityLimitReachedAsync(userId, activityType, date))
        {
            throw new DomainException($"Достигнут лимит событий {activityType.Name} такого типа на дату {date}");
        }
    }
}
