using System;
using System.Collections.Generic;
using System.Text;

namespace DailyTracker.Domain.Entities.Activities;

/// <summary>
/// Фабрика для создания событий
/// </summary>
public interface IActivityFactory
{
    Task<WeightActivity> CreateWeightActivityAsync(
        Guid userId,
        ActivityType activityType,
        DateTime date,
        double weight,
        CancellationToken cancellationToken = default);

    Task<DurationActivity> CreateDurationActivityAsync(
        Guid userId,
        ActivityType activityType,
        DateTime date,
        TimeSpan duration,
        CancellationToken cancellationToken = default);

    Task<TimeOfDayActivity> CreateTimeActivityAsync(
        Guid userId,
        ActivityType activityType,
        DateTime date,
        CancellationToken cancellationToken = default);
}
