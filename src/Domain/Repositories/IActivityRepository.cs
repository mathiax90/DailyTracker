using System;
using System.Collections.Generic;
using System.Text;

namespace DailyTracker.Domain.Repositories;

public interface IActivityRepository
{
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
