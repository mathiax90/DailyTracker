using static DailyTracker.Domain.Entities.ActivityType;

namespace DailyTracker.Domain.Repositories;

public interface IActivityTypeRepository
{
    Task<ActivityType?> GetByIdAsync(Guid userId, int id, CancellationToken ct = default);

    Task AddAsync(ActivityType activityType);
    void Remove(ActivityTypeDeleteToken deleteToken);
}
