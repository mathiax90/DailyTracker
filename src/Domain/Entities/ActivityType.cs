using DailyTracker.Domain.Constants;

namespace DailyTracker.Domain.Entities;

/// <summary>
/// Тип активности/события
/// </summary>
public class ActivityType : BaseAuditableEntity
{
    public string Name { get; set; } = "";
    public MetricType MetricType { get; private set; }
    /// <summary>
    /// Признак что объект создан разработчиками, а не пользователем
    /// </summary>
    public bool IsSystem { get; private set; } = false;
    public Project? Project { get; private set; }
    
    private int _dayLimit = 0;

    /// <summary>
    /// Лимит событий в день
    /// </summary>
    public int DayLimit
    {
        get
        { 
            return _dayLimit;
        }

        private set
        {
            if (value < 0)
                throw new DomainException("Лимит событий в день не может быть меньше нуля.");
            _dayLimit = value;
        }
    }

    protected ActivityType()
    {
    }

    /// <summary>
    /// Создаёт тип события
    /// </summary>
    /// <param name="name">Название</param>
    /// <param name="metricType">Тип метрики</param>
    /// <param name="dayLimit">Лимит события в день</param>
    public ActivityType(string name, MetricType metricType, int dayLimit = 0)
    {
        Name = name;
        MetricType = metricType;
        DayLimit = dayLimit;
    }

    /// <summary>
    /// Создаёт тип события по проекту
    /// </summary>
    /// <param name="name">Название</param>
    /// <param name="metricType">Тип метрики</param>
    /// <param name="dayLimit">Лимит события в день</param>
    /// <param name="project">Проект</param>
    public ActivityType(string name, MetricType metricType, Project project, int dayLimit = 0)
    {
        Name = name;
        MetricType = metricType;
        DayLimit = dayLimit;
        Project = project;
    }

    /// <summary>
    /// Создаёт системный тип события (встроенный в систему)
    /// </summary>
    /// <param name="id"></param>
    /// <param name="name"></param>
    /// <param name="metricType"></param>
    /// <param name="dayLimit"></param>
    public ActivityType(int sysActTypeId, string name, MetricType metricType, int dayLimit = 0) : this(name, metricType, dayLimit)
    {
        Id = sysActTypeId;
        IsSystem = true;
    }
}
