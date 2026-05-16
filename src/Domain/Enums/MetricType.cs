namespace DailyTracker.Domain.Enums;

/// <summary>
/// Тип метрики события (единица измерения связанная с значением и событием)
/// </summary>
public enum MetricType
{
    Duration = 0,
    Weight = 1,
    TimeOfDay = 2,
    Length = 3
}
