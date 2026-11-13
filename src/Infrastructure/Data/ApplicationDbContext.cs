using System.Reflection;
using DailyTracker.Application.Common.Interfaces;
using DailyTracker.Domain.Entities;
using DailyTracker.Domain.Entities.Activities;
using DailyTracker.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DailyTracker.Infrastructure.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<TodoList> TodoLists => Set<TodoList>();
    public DbSet<TodoItem> TodoItems => Set<TodoItem>();
    public DbSet<Project> Projects => Set<Project>();

    internal DbSet<ActivityType> ActivityTypesSet => Set<ActivityType>();
    internal DbSet<Activity> ActivitiesSet => Set<Activity>();
    IQueryable<Activity> IApplicationDbContext.Activities => ActivitiesSet.AsNoTracking();
    IQueryable<ActivityType> IApplicationDbContext.ActivityTypes => ActivityTypesSet.AsNoTracking();
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
