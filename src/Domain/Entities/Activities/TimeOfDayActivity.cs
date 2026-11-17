using System.Globalization;

namespace DailyTracker.Domain.Entities.Activities;

/// <summary>
/// Активность/событие
/// </summary>
public class TimeOfDayActivity : Activity
{

    public override string MetricValue => $"{ActivityDate.ToString(new CultureInfo("ru-RU"))}";
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

    internal override void Update(DateTime activityDate, string metric)
    {
        DateTime dt = new DateTime();
        if (!string.IsNullOrEmpty(metric))
            dt = ActivityService.ParseDateTime(metric);
        else
            dt = activityDate;

        this.ActivityDate = dt;
    }
}
