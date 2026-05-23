using System.Diagnostics;

namespace DailyTracker.Domain.Entities.Activities;

/// <summary>
/// Активность/событие
/// </summary>
public class DurationActivity : Activity
{
    public Duration Duration { get; private set; } = new Duration();
    protected DurationActivity()
    {
        
    }
    internal DurationActivity(Guid userId, ActivityType activityType, DateTime activityDate, Duration duration) : base(userId, activityType, activityDate)
    {
        if (activityType.MetricType != MetricType.Duration)
        {
            throw new DomainException("Невернтый тип метрики для данного события.");
        }
        Duration = duration;
    }
}
