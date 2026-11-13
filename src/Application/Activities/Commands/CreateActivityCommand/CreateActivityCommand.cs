using DailyTracker.Application.Common.Interfaces;
using DailyTracker.Domain.Entities;
using DailyTracker.Domain.Entities.Activities;
using DailyTracker.Domain.Repositories;

namespace DailyTracker.Application.Activities.Commands.CreateActivityCommand;

public record CreateActivityCommand(Guid UserId, int ActivityTypeId, DateTime ActivityDate, string Metric) : IRequest<int>;

public class CreateActivityCommandValidator : AbstractValidator<CreateActivityCommand>
{
    public CreateActivityCommandValidator()
    {
    }
}

public class CreateActivityCommandHandler : IRequestHandler<CreateActivityCommand, int>
{
    private readonly IApplicationDbContext _context;
    private readonly IActivityRepository _activityRepository;
    private readonly IActivityTypeRepository _activityTypeRepository;

    public CreateActivityCommandHandler(IApplicationDbContext context,
        IActivityRepository activityRepository,
        IActivityTypeRepository activityTypeRepository)
    {
        _context = context;
        _activityRepository = activityRepository;
        _activityTypeRepository = activityTypeRepository;
    }

    public async Task<int> Handle(CreateActivityCommand request, CancellationToken cancellationToken)
    {
        ActivityFactory _factory = new ActivityFactory(_activityRepository);

        ActivityType? activityType = await _activityTypeRepository.GetByIdAsync(request.ActivityTypeId);

        Guard.Against.Null(activityType);

        var activity = await _factory.CreateActivity(request.UserId, activityType, request.ActivityDate, request.Metric);

        await _activityRepository.AddAsync(activity);
        return await _context.SaveChangesAsync(cancellationToken);

        //return activity.Id;
    }
}
