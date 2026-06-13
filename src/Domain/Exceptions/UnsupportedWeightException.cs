namespace DailyTracker.Domain.Exceptions;

public class UnsupportedWeightException : Exception
{
    public UnsupportedWeightException()
        : base("Вес должен быть больше или равен нулю.")
    {
    }
}
