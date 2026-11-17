using DailyTracker.Application.Common.Interfaces;
using DailyTracker.Domain.Entities;
using DailyTracker.Domain.Enums;
using DailyTracker.Domain.Repositories;

namespace DailyTracker.Application.ActivityTypes.Commands.CreateActivityTypeCommand;

public record CreateActivityTypeCommand(Guid UserId, string Name, MetricType MetricType, int? ProjectId, int? DayLimit) : IRequest<int>;

public class CreateActivityTypeCommandCommandValidator : AbstractValidator<CreateActivityTypeCommand>
{
    public CreateActivityTypeCommandCommandValidator()
    {
    }
}

public class CreateActivityTypeCommandHandler : IRequestHandler<CreateActivityTypeCommand, int>
{
    private readonly IApplicationDbContext _context;
    private readonly IActivityTypeRepository _activityTypeRepository;
    private readonly IUnitOfWork _uow;

    public CreateActivityTypeCommandHandler(IApplicationDbContext context,
        IActivityTypeRepository activityTypeRepository,
        IUnitOfWork uow)
    {
        _context = context;
        _activityTypeRepository = activityTypeRepository;
        _uow = uow;
    }

    public async Task<int> Handle(CreateActivityTypeCommand request, CancellationToken cancellationToken)
    {
        ActivityType? at = null;
        int dayLimit = request.DayLimit.HasValue ? request.DayLimit.Value : 0;
        
        if (request.ProjectId.HasValue)
        {
            Project? proj = _context.Projects.FirstOrDefault(x => x.Id == request.ProjectId.Value);

            Guard.Against.Null(proj);

            at = new ActivityType(request.UserId, request.Name, request.MetricType, proj.Id, dayLimit);
        }
        else
        {
            at = new ActivityType(request.UserId, request.Name, request.MetricType, dayLimit);
        }

        await _activityTypeRepository.AddAsync(at);
        
        await _uow.SaveChangesAsync();

        return at.Id;
    }
}
