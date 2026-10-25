namespace DailyTracker.Domain.ValueObjects;

public class Weight : ValueObject
{
    public Weight(decimal val)
    {
        if (val < 0) throw new UnsupportedWeightException();
        Value = val;
    }

    public decimal Value { get; private set; }
    
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
