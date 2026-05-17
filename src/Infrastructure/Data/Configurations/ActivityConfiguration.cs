using DailyTracker.Domain.Entities.Activities;
using DailyTracker.Domain.Enums;
using DailyTracker.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Activity = DailyTracker.Domain.Entities.Activities.Activity;

namespace DailyTracker.Infrastructure.Data.Configurations;

public class ActivityConfiguration : IEntityTypeConfiguration<Activity>
{
    public void Configure(EntityTypeBuilder<Activity> builder)
    {
        builder.ToTable("Activities");

        builder.HasDiscriminator<MetricType>("ActivityKind")
            .HasValue<WeightActivity>(MetricType.Weight)
            .HasValue<DurationActivity>(MetricType.Duration)
            .HasValue<TimeOfDayActivity>(MetricType.TimeOfDay);

        builder.HasOne(a => a.ActivityType)
            .WithMany()
            .HasForeignKey("ActivityTypeId")
            .IsRequired();

        builder.Property(a => a.ActivityDate).IsRequired();

        // 3. Настройка Value Object для WeightActivity
        builder.OwnsOne(typeof(Weight), "Weight", wp =>
        {
            wp.Property("Value")
              .HasColumnName("WeightValue");
        });

        // 4. Настройка свойств для DurationActivity
        builder.Property(typeof(TimeSpan), "Duration")
           .HasColumnName("Duration");
    }
}
