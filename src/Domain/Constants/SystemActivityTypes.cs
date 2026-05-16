using System;
using System.Collections.Generic;
using System.Text;

namespace DailyTracker.Domain.Constants;

public static class SystemActivityTypes
{
    /// <summary>
    /// Идентификатор типа события Пробуждение
    /// </summary>
    public static readonly Guid WakeUp = Guid.Parse("A1B2C3D4-E5F6-7A8B-9C0D-E1F2A3B4C5D0");
    /// <summary>
    /// Идентификатор типа события Вес тела
    /// </summary>
    public static readonly Guid BodyWeight = Guid.Parse("A1B2C3D4-E5F6-7A8B-9C0D-E1F2A3B4C5D1");
}
