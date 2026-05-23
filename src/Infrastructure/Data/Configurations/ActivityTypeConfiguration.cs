using DailyTracker.Domain.Constants;
using DailyTracker.Domain.Entities;
using DailyTracker.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DailyTracker.Infrastructure.Data.Configurations;

public class ActivityTypeConfiguration : IEntityTypeConfiguration<ActivityType>
{
    public void Configure(EntityTypeBuilder<ActivityType> builder)
    {
        // Наполняем базу системными данными (Data Seeding)
        builder.HasData(new ActivityType(SystemActivityTypes.WakeUp,
            "Время пробуждения",
            MetricType.TimeOfDay,
            1),
            new ActivityType(SystemActivityTypes.BodyWeight,
            "Вес тела",
            MetricType.Weight,
            3)
        );
    }
}
