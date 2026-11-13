using System;
using System.Collections.Generic;
using System.Text;

namespace DailyTracker.Shared;

public record ActivityDto(Guid UserId, int Id, int ActivityTypeId, string ActivityTypeName, DateTime ActivityDate, string MetricValue);
