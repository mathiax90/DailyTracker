using DailyTracker.Application.Common.Interfaces;
using DailyTracker.Shared;

namespace DailyTracker.Application.ActivityTypes.Queries.ActivityTypeQuery;

public record ActivityTypeQuery(Guid UserId, int? ActivityTypeId) : IRequest<List<ActivityTypeDto>>;
public class ActivityTypeQueryValidator : AbstractValidator<ActivityTypeQuery>
{
    public ActivityTypeQueryValidator()
    {
        // UserId обязателен в любом случае
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("Идентификатор пользователя (UserId) обязателен.");
    }
}

public class ActivityTypeQueryHandler : IRequestHandler<ActivityTypeQuery, List<ActivityTypeDto>>
{
    private readonly IApplicationDbContext _context;

    public ActivityTypeQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<ActivityTypeDto>> Handle(ActivityTypeQuery request, CancellationToken cancellationToken)
    {
        List<ActivityTypeDto>? dtos = null;
        dtos = await _context.ActivityTypes.Where(x => (x.UserId == request.UserId || x.UserId == null)
        && (request.ActivityTypeId == null || (x.Id == request.ActivityTypeId)))
            .Select(x => new ActivityTypeDto(
                x.UserId,
                x.Id,
                x.Name,
                (int)x.MetricType,
                x.Project != null ? x.Project.Id : null,
                x.Project != null ? x.Project.Name : null))
            .ToListAsync(cancellationToken);

        return dtos;
    }
}
