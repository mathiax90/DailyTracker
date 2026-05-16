namespace DailyTracker.Domain.Entities;

/// <summary>
/// Проект
/// </summary>
public class Project : BaseAuditableEntity
{
    public string Name { get; set; } = "";
}
