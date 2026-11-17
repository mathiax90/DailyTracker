
namespace DailyTracker.Domain.Entities.Activities;

/// <summary>
/// Активность/событие
/// </summary>
public class DurationActivity : Activity
{
    public Duration Duration { get; private set; } = new Duration();
    public override string MetricValue => $"{(int)Duration.Value.TotalHours:D2}:{Duration.Value.Minutes:D2} ({(int)Duration.Value.TotalMinutes} мин.)";
    protected DurationActivity()
    {
        
    }
    internal DurationActivity(Guid userId, ActivityType activityType, DateTime activityDate, Duration duration) 
        : base(userId, activityType, activityDate)
    {
        if (activityType.MetricType != MetricType.Duration)
        {
            throw new DomainException("Невернтый тип метрики для данного события.");
        }
        Duration = duration;
    }

    internal override void Update(DateTime activityDate, string metric)
    {
        base.Update(activityDate, metric);
        Duration = Duration.Parse(metric);
    }
}
