using DailyTracker.Domain.Entities.Activities;
using DailyTracker.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DailyTracker.Infrastructure.Data.Configurations;

public class DurationActivityConfiguration : IEntityTypeConfiguration<DurationActivity>
{
    public void Configure(EntityTypeBuilder<DurationActivity> builder)
    {
        builder.Property(w => w.Duration)
            .HasColumnName("DurationValue")
            .HasConversion(
                duration => (TimeSpan?)duration.Value,
                value => value.HasValue ? new Duration(value.Value) : new Duration()
            )
            .IsRequired(true); //проверить нормально ли работает
    }

}
