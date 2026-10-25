using System;
using System.Collections.Generic;
using System.Text;

namespace DailyTracker.Domain.Entities.Activities;

/// <summary>
/// Фабрика для создания событий
/// </summary>
public interface IActivityFactory
{
    /// <summary>
    /// Создание Activity (События) с парсингом значения (метрики)
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="activityType"></param>
    /// <param name="date"></param>
    /// <param name="metricValue"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Activity> CreateActivity(Guid userId, ActivityType activityType, DateTime date, string metricValue, CancellationToken cancellationToken = default);
    //Task<WeightActivity> CreateWeightActivityAsync(
    //    Guid userId,
    //    ActivityType activityType,
    //    DateTime date,
    //    double weight,
    //    CancellationToken cancellationToken = default);

    //Task<DurationActivity> CreateDurationActivityAsync(
    //    Guid userId,
    //    ActivityType activityType,
    //    DateTime date,
    //    TimeSpan duration,
    //    CancellationToken cancellationToken = default);

    //Task<TimeOfDayActivity> CreateTimeActivityAsync(
    //    Guid userId,
    //    ActivityType activityType,
    //    DateTime date,
    //    CancellationToken cancellationToken = default);
}
