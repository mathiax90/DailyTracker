namespace DailyTracker.Domain.ValueObjects;

public class Weight : ValueObject
{
    public Weight(double val)
    {
        if (val <= 0) throw new UnsupportedWeightException();
        Value = val;
    }

    public double Value { get; private set; }
    
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
