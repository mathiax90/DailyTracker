using DailyTracker.Domain.Entities;
using DailyTracker.Domain.Entities.Activities;
using DailyTracker.Domain.Repositories;
using DailyTracker.Infrastructure.Data;

namespace DailyTracker.Infrastructure.Repositories;

public class ActivityRepository : IActivityRepository
{
    private readonly ApplicationDbContext _context;

    public ActivityRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Activity activity)
    {
        await _context.ActivitiesSet.AddAsync(activity);
    }

    public async Task<Activity?> GetByIdAsync(Guid userId, int activityId, CancellationToken ct = default)
    {
        return _context.ActivitiesSet.FirstOrDefault(x => x.UserId == userId && x.Id == activityId);
    }

    public async Task<bool> IsActivityLimitReachedAsync(Guid userId, ActivityType activityType, DateTime date, CancellationToken cancellationToken = default)
    {
        int sameDayActivitiesCount = _context.ActivitiesSet.Count(x => x.UserId == userId
            && x.ActivityDate.Date == date.Date
            && x.ActivityType == activityType);

        return sameDayActivitiesCount >= activityType.DayLimit;
    }

    public void Remove(Activity a)
    {
        _context.ActivitiesSet.Remove(a);
    }
}
