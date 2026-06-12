namespace DailyTracker.Domain.ValueObjects;

public class Duration : ValueObject
{
    public Duration(TimeSpan val)
    {
        if (val <= TimeSpan.Zero) throw new DomainException("Длительность события не может быть меньше или ровна нуля.");
        Value = val;
    }

    /// <summary>
    /// Длительность длинною в 1 секунду
    /// </summary>
    public Duration()
    {
        Value = TimeSpan.FromSeconds(1);
    }

    public TimeSpan Value { get; private set; }
    
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
