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
    private readonly IUnitOfWork _uow;

    public CreateActivityCommandHandler(IApplicationDbContext context,
        IActivityRepository activityRepository,
        IActivityTypeRepository activityTypeRepository,
        IUnitOfWork uow)
    {
        _context = context;
        _activityRepository = activityRepository;
        _activityTypeRepository = activityTypeRepository;
        _uow = uow;
    }

    public async Task<int> Handle(CreateActivityCommand request, CancellationToken cancellationToken)
    {
        ActivityService _factory = new ActivityService(_activityRepository);

        ActivityType? activityType = await _activityTypeRepository.GetByIdAsync(request.UserId, request.ActivityTypeId);

        Guard.Against.Null(activityType);

        var activity = await _factory.CreateActivity(request.UserId, activityType, request.ActivityDate, request.Metric);

        await _activityRepository.AddAsync(activity);
        
        await _uow.SaveChangesAsync(cancellationToken);

        return activity.Id;
    }
}
