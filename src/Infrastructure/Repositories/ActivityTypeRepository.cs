using System;
using System.Collections.Generic;
using System.Text;
using DailyTracker.Domain.Entities;
using DailyTracker.Domain.Repositories;
using DailyTracker.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DailyTracker.Infrastructure.Repositories;

public class ActivityTypeRepository : IActivityTypeRepository
{
    private readonly ApplicationDbContext _context;

    public ActivityTypeRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ActivityType?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return _context.ActivityTypesSet.FirstOrDefault(x => x.Id == id);
    }

    public async Task AddAsync(ActivityType activityType)
    {
        await _context.ActivityTypesSet.AddAsync(activityType);
    }
}
