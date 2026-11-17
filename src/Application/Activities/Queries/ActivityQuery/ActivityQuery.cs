using DailyTracker.Application.Common.Interfaces;
using DailyTracker.Shared;
namespace DailyTracker.Application.Activities.Queries.ActivityQuery;

public record ActivityQuery(Guid UserId, int? ActivityId = null, DateTime? FromDate = null, DateTime? ToDate = null) : IRequest<List<ActivityDto>>;

public class ActivityQueryValidator : AbstractValidator<ActivityQuery>
{
    public ActivityQueryValidator()
    {
        // UserId обязателен в любом случае
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("Идентификатор пользователя (UserId) обязателен.");

        // Проверяем, что заполнено хотя бы одно из условий: или ActivityId, или обе даты
        RuleFor(x => x)
            .Must(x => x.ActivityId.HasValue || (x.FromDate.HasValue && x.ToDate.HasValue))
            .WithMessage("Необходимо указать либо ActivityId, либо обе даты (FromDate и ToDate).")
            // Если это общее условие нарушено, остальные правила для дат проверять нет смысла
            .DependentRules(() =>
            {
                // Правила выполняются только если ActivityId НЕ указан (значит, валидируем даты)
                When(x => !x.ActivityId.HasValue, () =>
                {
                    RuleFor(x => x.FromDate)
                        .NotEmpty().WithMessage("Начальная дата (FromDate) обязательна.");

                    RuleFor(x => x.ToDate)
                        .NotEmpty().WithMessage("Конечная дата (ToDate) обязательна.")
                        .GreaterThanOrEqualTo(x => x.FromDate!.Value).WithMessage("Конечная дата не может быть меньше начальной.");

                    RuleFor(x => x)
                        .Must(x => BeLessThanAMonth(x.FromDate!.Value, x.ToDate!.Value))
                        .WithMessage("Период между датами не должен превышать один месяц.");
                });
            });
    }

    private bool BeLessThanAMonth(DateTime fromDate, DateTime toDate)
    {
        // Добавляем 1 месяц к начальной дате и проверяем, 
        // чтобы конечная дата не выходила за этот предел
        return toDate <= fromDate.AddMonths(1);
    }
}

public class ActivityQueryHandler : IRequestHandler<ActivityQuery, List<ActivityDto>>
{
    private readonly IApplicationDbContext _context;

    public ActivityQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<ActivityDto>> Handle(ActivityQuery request, CancellationToken cancellationToken)
    {
        List<ActivityDto>? activities = null;

        var query = _context.Activities.Where(x => x.UserId == request.UserId);

        bool validRequest = true;

        if (request.ActivityId.HasValue)
        {
            query = query.Where(x => x.Id == request.ActivityId.Value);
        }
        else if (request.FromDate.HasValue && request.ToDate.HasValue)
        {
            query = query.Where(x => x.ActivityDate >= request.FromDate.Value
                                  && x.ActivityDate < request.ToDate.Value);
        }
        else
        {
            validRequest = false;
        }

        if (validRequest)
        {
            activities = await query
            .Select(x => new ActivityDto(
                x.UserId,
                x.Id,
                x.ActivityType.Id,
                x.ActivityType.Name,
                x.ActivityDate,
                x.MetricValue))
            .ToListAsync(cancellationToken);
        }
        else
        {
            activities = new List<ActivityDto>();
        }

        return activities;
    }
}
