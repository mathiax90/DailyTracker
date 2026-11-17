using DailyTracker.Application.Common.Interfaces;
using DailyTracker.Domain.Entities.Activities;
using DailyTracker.Domain.Repositories;

namespace DailyTracker.Application.Activities.Commands.UpdateActivity;

public record UpdateActivityCommand(Guid UserId, int ActivityId, DateTime ActivityDate, string Metric) : IRequest<int>;

public class UpdateActivityCommandValidator : AbstractValidator<UpdateActivityCommand>
{
    public UpdateActivityCommandValidator()
    {
    }
}

public class UpdateActivityCommandHandler : IRequestHandler<UpdateActivityCommand, int>
{
    private readonly IApplicationDbContext _context;
    private readonly IActivityRepository _activityRepository;
    private readonly IUnitOfWork _uow;

    public UpdateActivityCommandHandler(IApplicationDbContext context,
        IActivityRepository activityRepository,
        IUnitOfWork uow)
    {
        _context = context;
        _activityRepository = activityRepository;
        _uow = uow;
    }

    public async Task<int> Handle(UpdateActivityCommand request, CancellationToken cancellationToken)
    {
        Activity? activity = await _activityRepository.GetByIdAsync(request.UserId, request.ActivityId);
        Guard.Against.Null(activity);

        ActivityService activityService = new ActivityService(_activityRepository);

        await activityService.UpdateAsync(activity, request.ActivityDate, request.Metric);

        await _uow.SaveChangesAsync(cancellationToken);

        return activity.Id;
    }
}
