namespace DailyTracker.Domain.Exceptions;

public class UnsupportedWeightException : Exception
{
    public UnsupportedWeightException()
        : base("Weight cannot be less than or equal to zero.")
    {
    }
}
