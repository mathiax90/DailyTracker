namespace DailyTracker.Shared;

public record ActivityTypeDto(Guid? UserId, int Id, string Name, int MetricType, int? ProjectId, string? ProjectName);
