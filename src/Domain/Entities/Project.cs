namespace DailyTracker.Domain.Entities;

/// <summary>
/// Проект
/// </summary>
public class Project : BaseAuditableEntity
{
    public Project(int userId, string name)
    {
        UserID = userId;
        Name = name;
    }

    private Project()
    {
        
    }
    public int UserID { get; private set; }
    public string Name { get; private set; } = "";
}
