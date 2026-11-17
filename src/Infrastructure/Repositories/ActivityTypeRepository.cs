using DailyTracker.Domain.Entities;
using DailyTracker.Domain.Repositories;
using DailyTracker.Infrastructure.Data;

namespace DailyTracker.Infrastructure.Repositories;

public class ActivityTypeRepository : IActivityTypeRepository
{
    private readonly ApplicationDbContext _context;

    public ActivityTypeRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ActivityType?> GetByIdAsync(Guid userId, int activityTypeId, CancellationToken ct = default)
    {
        return _context.ActivityTypesSet.FirstOrDefault(x => (x.UserId == userId || x.UserId == null)
        && x.Id == activityTypeId);
    }

    public async Task AddAsync(ActivityType activityType)
    {
        await _context.ActivityTypesSet.AddAsync(activityType);
    }
}
