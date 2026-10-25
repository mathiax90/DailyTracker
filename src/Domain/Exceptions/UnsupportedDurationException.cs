namespace DailyTracker.Domain.Exceptions;

public class UnsupportedDurationException : Exception
{
    public UnsupportedDurationException()
        : base("Минимальная длительность составляет одну секунду. Максимальная длительность - 24 часа.")
    {
    }
}
