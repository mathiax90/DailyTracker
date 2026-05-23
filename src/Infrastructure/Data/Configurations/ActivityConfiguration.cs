using DailyTracker.Domain.Entities.Activities;
using DailyTracker.Domain.Enums;
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
            .HasValue<Activity>(MetricType.Base)
            .HasValue<WeightActivity>(MetricType.Weight)
            .HasValue<DurationActivity>(MetricType.Duration)
            .HasValue<TimeOfDayActivity>(MetricType.TimeOfDay);

        builder.HasOne(a => a.ActivityType)
            .WithMany()
            .HasForeignKey("ActivityTypeId")
            .IsRequired();

        builder.Property(a => a.ActivityDate).IsRequired();
    }
}
