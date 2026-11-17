using System.Globalization;
using DailyTracker.Domain.Repositories;

namespace DailyTracker.Domain.Entities.Activities;

public class ActivityService
{
    private readonly IActivityRepository _activityRepository;

    

    public ActivityService(IActivityRepository activityRepository)
    {
        _activityRepository = activityRepository;
    }

    public async Task<Activity> CreateActivity(Guid userId, ActivityType activityType, DateTime date, string? metricValue, CancellationToken cancellationToken = default)
    {
        switch (activityType.MetricType)
        {
            case MetricType.Weight:
                return await CreateWeightActivityAsync(userId, activityType, date, Weight.Parse(metricValue), cancellationToken);
            case MetricType.Duration:
                return await CreateDurationActivityAsync(userId, activityType, date, Duration.Parse(metricValue), cancellationToken);
            case MetricType.TimeOfDay:
                DateTime dt = new DateTime();
                if (!string.IsNullOrEmpty(metricValue))
                    dt = ParseDateTime(metricValue);
                else
                    dt = date;
                return await CreateTimeActivityAsync(userId, activityType, dt, cancellationToken);
            default:
                throw new DomainException("Неизвестный тип метрики.");
        }
    }

    internal static DateTime ParseDateTime(string? metricValue)
    {
        CultureInfo ruCI = new CultureInfo("ru-RU");

        if (DateTime.TryParseExact(metricValue, "H:mm", ruCI, DateTimeStyles.None, out DateTime parsedDate))
        {
            return parsedDate;
        }
        else if (DateTime.TryParse(metricValue, ruCI, out DateTime parsedDate1))
        {
            return parsedDate1;
        }
            
        throw new DomainException("Ошибка парсинга даты и/или времени.");
    }

    internal async Task<DurationActivity> CreateDurationActivityAsync(Guid userId, ActivityType activityType, DateTime date, Duration duration, CancellationToken cancellationToken = default)
    {
        await CheckLimitsAsync(userId, activityType, date);
        return new DurationActivity(userId, activityType, date, duration);
    }

    internal async Task<TimeOfDayActivity> CreateTimeActivityAsync(Guid userId, ActivityType activityType, DateTime date, CancellationToken cancellationToken = default)
    {
        await CheckLimitsAsync(userId, activityType, date);
        return new TimeOfDayActivity(userId, activityType, date);
    }

    internal async Task<WeightActivity> CreateWeightActivityAsync(Guid userId, ActivityType activityType, DateTime date, Weight weight, CancellationToken cancellationToken = default)
    {
        await CheckLimitsAsync(userId, activityType, date);
        return new WeightActivity(userId, activityType, date, weight);
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

    public async Task UpdateAsync(Activity activity, DateTime activityDate, string metric)
    {
        if (activity.ActivityDate.Date != activityDate.Date)
            await CheckLimitsAsync(activity.UserId, activity.ActivityType, activityDate);
        activity.Update(activityDate, metric);
    }
}
