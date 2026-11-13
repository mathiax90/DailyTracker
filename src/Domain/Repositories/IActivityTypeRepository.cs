namespace DailyTracker.Domain.Repositories;

public interface IActivityTypeRepository
{
    Task<ActivityType?> GetByIdAsync(int id, CancellationToken ct = default);
}
