using System.Globalization;

namespace DailyTracker.Domain.ValueObjects;

public class Weight : ValueObject
{
    public Weight(decimal val)
    {
        if (val < 0) throw new UnsupportedWeightException();
        Value = val;
    }

    public decimal Value { get; private set; }

    internal static Weight Parse(string? metricValue)
    {
        if (Decimal.TryParse(metricValue?.Replace(",", "."), CultureInfo.InvariantCulture, out decimal weight))
            return new Weight(weight);
        else
            throw new DomainException("Ошибка парсинга веса.");
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
