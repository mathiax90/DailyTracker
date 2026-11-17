using Ardalis.GuardClauses;

namespace DailyTracker.Domain.Entities;

/// <summary>
/// Тип активности/события
/// </summary>
public class ActivityType : BaseAuditableEntity
{
    public string Name { get; set; } = "";
    public MetricType MetricType { get; private set; }
    public Guid? UserId { get; private set; }
    /// <summary>
    /// Признак что объект создан разработчиками, а не пользователем
    /// </summary>
    public bool IsSystem => !UserId.HasValue;
    public int? ProjectId { get; private set; }
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
    /// <param name="userId">Идентификатор пользователя</param>
    /// <param name="name">Название</param>
    /// <param name="metricType">Тип метрики</param>
    /// <param name="dayLimit">Лимит события в день</param>
    public ActivityType(Guid userId, string name, MetricType metricType, int dayLimit)
    {
        Guard.Against.Default(userId);
        Guard.Against.NullOrWhiteSpace(name);
        Guard.Against.NegativeOrZero(dayLimit);

        UserId = userId;
        Name = name;
        MetricType = metricType;
        DayLimit = dayLimit;
    }

    /// <summary>
    /// Создаёт тип события по проекту
    /// </summary>
    /// <param name="userId">Идентификатор пользователя</param>
    /// <param name="name">Название</param>
    /// <param name="metricType">Тип метрики</param>
    /// <param name="dayLimit">Лимит события в день</param>
    /// <param name="project">Проект</param>
    public ActivityType(Guid userId, string name, MetricType metricType, int projectId, int dayLimit):
        this(userId, name, metricType, dayLimit)
    {
        ProjectId = projectId;
    }

    /// <summary>
    /// Создаёт системный тип события (встроенный в систему)
    /// </summary>
    /// <param name="sysActTypeId"></param>
    /// <param name="userId"></param>
    /// <param name="name"></param>
    /// <param name="metricType"></param>
    /// <param name="dayLimit"></param>
    public ActivityType(int sysActTypeId, string name, MetricType metricType, int dayLimit = 0)
    {
        Guard.Against.NullOrWhiteSpace(name);
        Guard.Against.NegativeOrZero(dayLimit);

        Name = name;
        MetricType = metricType;
        DayLimit = dayLimit;

        if (sysActTypeId >= 0)
        {
            throw new DomainException("Системные типы событий должны иметь отрицательный идентификатор.");
        }

        Id = sysActTypeId;
    }

    public ActivityTypeDeleteToken Delete()
    {
        if (IsSystem)
            throw new DomainException("Запрещено удалять системные события!");
        return new ActivityTypeDeleteToken(this);
    }

    public void Rename(string name)
    {
        Guard.Against.NullOrWhiteSpace(name);
        this.Name = name;
    }

    /// <summary>
    /// Токен подтверждающий возможность удаления
    /// </summary>
    public sealed class ActivityTypeDeleteToken
    {
        public ActivityType ActivityType { get; private set; }
        internal ActivityTypeDeleteToken(ActivityType activityType)
        {
            ActivityType = activityType;
        }
    }
}
