using System.Globalization;
using System.Text.RegularExpressions;
using DailyTracker.Domain.Repositories;

namespace DailyTracker.Domain.Entities.Activities;

public class ActivityFactory : IActivityFactory
{
    private readonly IActivityRepository _activityRepository;

    

    public ActivityFactory(IActivityRepository activityRepository)
    {
        _activityRepository = activityRepository;
    }

    public async Task<Activity> CreateActivity(Guid userId, ActivityType activityType, DateTime date, string? metricValue, CancellationToken cancellationToken = default)
    {
        switch (activityType.MetricType)
        {
            case MetricType.Weight:
                decimal weight = ParseWeight(metricValue);
                return await CreateWeightActivityAsync(userId, activityType, date, weight, cancellationToken);
            case MetricType.Duration:
                TimeSpan durationTimeSpan = ParseDuration(metricValue);
                return await CreateDurationActivityAsync(userId, activityType, date, durationTimeSpan, cancellationToken);
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

    private DateTime ParseDateTime(string? metricValue)
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

    private TimeSpan ParseDuration(string? metricValue)
    {
        if (!string.IsNullOrEmpty(metricValue))
        {
            Regex regex = new Regex(@"^((0[0-9])|(1[1-9])|(2[0-3])):[0-5]\d:[0-5]\d$");
            if (regex.IsMatch(metricValue))
            {
                return TimeSpan.Parse(metricValue);
            }
            else if (int.TryParse(metricValue, out int durationInMinutes))
            {
                return TimeSpan.FromMinutes(durationInMinutes);
            }
        }

        throw new DomainException("Ошибка парсинга длительности.");
    }

    private decimal ParseWeight(string? metricValue)
    {
        if (Decimal.TryParse(metricValue?.Replace(",", "."), CultureInfo.InvariantCulture, out decimal weight))
            return weight;
        else
            throw new DomainException("Ошибка парсинга веса.");
    }

    internal async Task<DurationActivity> CreateDurationActivityAsync(Guid userId, ActivityType activityType, DateTime date, TimeSpan duration, CancellationToken cancellationToken = default)
    {
        Duration d = new Duration(duration);
        await CheckLimitsAsync(userId, activityType, date);
        return new DurationActivity(userId, activityType, date, d);
    }

    internal async Task<TimeOfDayActivity> CreateTimeActivityAsync(Guid userId, ActivityType activityType, DateTime date, CancellationToken cancellationToken = default)
    {
        await CheckLimitsAsync(userId, activityType, date);
        return new TimeOfDayActivity(userId, activityType, date);
    }

    internal async Task<WeightActivity> CreateWeightActivityAsync(Guid userId, ActivityType activityType, DateTime date, decimal weight, CancellationToken cancellationToken = default)
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
