using System.Text.RegularExpressions;

namespace DailyTracker.Domain.ValueObjects;

public class Duration : ValueObject
{
    public Duration(TimeSpan val)
    {
        if (val <= TimeSpan.Zero || val > TimeSpan.FromHours(24)) throw new UnsupportedDurationException();
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

    internal static Duration Parse(string? metricValue)
    {
        if (!string.IsNullOrEmpty(metricValue))
        {
            Regex regex = new Regex(@"^((0[0-9])|(1[1-9])|(2[0-3])):[0-5]\d:[0-5]\d$");
            if (regex.IsMatch(metricValue))
            {
                return new Duration(TimeSpan.Parse(metricValue));
            }
            else if (int.TryParse(metricValue, out int durationInMinutes))
            {
                return new Duration(TimeSpan.FromMinutes(durationInMinutes));
            }
        }

        throw new DomainException("Ошибка парсинга длительности.");
    }
}
