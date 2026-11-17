using DailyTracker.Domain.Entities.Activities;

namespace DailyTracker.Domain.Repositories;

public interface IActivityRepository
{
    Task<Activity?> GetByIdAsync(Guid userId, int activityId, CancellationToken ct = default);
    Task AddAsync(Activity activity);
    
    /// <summary>
    /// Проверяет достигнут ли лимит события
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="activityType"></param>
    /// <param name="date"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<bool> IsActivityLimitReachedAsync(Guid userId, ActivityType activityType, DateTime date, CancellationToken cancellationToken = default);
}
