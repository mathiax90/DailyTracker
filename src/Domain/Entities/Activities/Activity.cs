namespace DailyTracker.Domain.Entities.Activities;

/// <summary>
/// Активность/событие
/// </summary>
public class Activity : BaseAuditableEntity
{
    public Guid UserId { get; private set; }
    public ActivityType ActivityType { get; private set; } = null!;
    public DateTime ActivityDate { get; private set; }
    protected Activity()
    {
        
    }
    internal Activity(Guid userId, ActivityType activityType, DateTime activityDate)
    {
        ActivityDate = activityDate;
    }
}
