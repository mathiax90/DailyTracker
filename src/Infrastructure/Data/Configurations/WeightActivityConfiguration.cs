using DailyTracker.Domain.Entities.Activities;
using DailyTracker.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DailyTracker.Infrastructure.Data.Configurations;

public class WeightActivityConfiguration : IEntityTypeConfiguration<WeightActivity>
{
    public void Configure(EntityTypeBuilder<WeightActivity> builder)
    {
        builder.Property(w => w.Weight)
            .HasColumnName("WeightValue")
            .HasConversion(
                // 1. Как сохранять в БД: приводим объект к nullable типу double?
                weight => (double?)weight.Value,

                // 2. Как читать из БД: если в базе NULL, возвращаем фиктивный null для EF, 
                // но для строк с типом 'Weight' там ВСЕГДА будет число, и создастся нормальный Weight.
                value => value.HasValue ? new Weight(value.Value) : new Weight(0)
            )
            .IsRequired(true); //проверить нормально ли работает
    }
}
