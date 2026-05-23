namespace DailyTracker.Domain.ValueObjects;

public class Duration : ValueObject
{
    public Duration(TimeSpan val)
    {
        if (val < TimeSpan.Zero) throw new DomainException("Недопустимая длительность события.");
        Value = val;
    }

    public Duration()
    {
        Value = TimeSpan.Zero;
    }

    public TimeSpan Value { get; private set; }
    
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
