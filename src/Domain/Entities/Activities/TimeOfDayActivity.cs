using System.Diagnostics;

namespace DailyTracker.Domain.Entities.Activities;

/// <summary>
/// Активность/событие
/// </summary>
public class TimeOfDayActivity : Activity
{
    protected TimeOfDayActivity()
    {
        
    }
    internal TimeOfDayActivity(Guid userId, ActivityType activityType, DateTime activityDate) : base(userId, activityType, activityDate)
    {
        if (activityType.MetricType != MetricType.TimeOfDay)
        {
            throw new DomainException("Невернтый тип метрики для данного события.");
        }
    }
}
