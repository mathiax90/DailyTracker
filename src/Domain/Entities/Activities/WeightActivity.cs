namespace DailyTracker.Domain.Entities.Activities;

/// <summary>
/// Активность/событие
/// </summary>
public class WeightActivity : Activity
{
    public Weight Weight { get; private set; } = new Weight(0);
    public override string MetricValue => $"{Weight.Value} кг";
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

    internal override void Update(DateTime activityDate, string metric)
    {
        base.Update(activityDate, metric);
        Weight = Weight.Parse(metric);
    }
}
