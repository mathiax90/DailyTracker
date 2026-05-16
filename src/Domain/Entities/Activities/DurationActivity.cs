using System.Diagnostics;

namespace DailyTracker.Domain.Entities.Activities;

/// <summary>
/// Активность/событие
/// </summary>
public class DurationActivity : Activity
{
    public TimeSpan Duration { get; private set; }
    protected DurationActivity()
    {
        
    }
    internal DurationActivity(Guid userId, ActivityType activityType, DateTime activityDate, TimeSpan duration) : base(userId, activityType, activityDate)
    {
        if (activityType.MetricType != MetricType.Duration)
        {
            throw new DomainException("Невернтый тип метрики для данного события.");
        }
        Duration = duration;
    }
}
