namespace DailyTracker.Domain.Entities.Activities;

/// <summary>
/// Активность/событие
/// </summary>
public class WeightActivity : Activity
{
    public Weight Weight { get; private set; } = new Weight(10);
    protected WeightActivity()
    {
        
    }
    internal WeightActivity(Guid userId, ActivityType activityType, DateTime activityDate, Weight weight) : base(userId, activityType, activityDate)
    {
        if (activityType.MetricType != MetricType.Weight)
        {
            throw new DomainException("Невернтый тип метрики для данного события.");
        }
        Weight = weight;
    }
}
