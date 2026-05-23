namespace DailyTracker.Domain.Enums;

/// <summary>
/// Тип метрики события (единица измерения связанная с значением и событием)
/// </summary>
public enum MetricType
{
    Base = 0,
    Duration = 1,
    Weight = 2,
    TimeOfDay = 3,
    Length = 4
}
