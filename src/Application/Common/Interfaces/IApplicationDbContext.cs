using DailyTracker.Domain.Entities;
using DailyTracker.Domain.Entities.Activities;

namespace DailyTracker.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<TodoList> TodoLists { get; }
    DbSet<TodoItem> TodoItems { get; }
    IQueryable<Activity> Activities { get; }
    IQueryable<ActivityType> ActivityTypes { get; }
    DbSet<Project> Projects { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
