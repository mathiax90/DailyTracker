using System.Xml.Linq;
using Ardalis.GuardClauses;

namespace DailyTracker.Domain.Entities.Activities;

/// <summary>
/// Активность/событие
/// </summary>
public class Activity : BaseAuditableEntity
{
    public Guid UserId { get; private set; }
    public ActivityType ActivityType { get; private set; } = null!;
    public DateTime ActivityDate { get; protected set; }

    public virtual string MetricValue => "";
    protected Activity()
    {
        
    }
    internal Activity(Guid userId, ActivityType activityType, DateTime activityDate)
    {
        Guard.Against.Default(userId);

        UserId = userId;
        ActivityType = activityType;
        ActivityDate = activityDate;
    }
}
