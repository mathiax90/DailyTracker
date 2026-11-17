using DailyTracker.Application.Common.Interfaces;
using DailyTracker.Domain.Entities;
using DailyTracker.Domain.Repositories;

namespace DailyTracker.Application.ActivityTypes.Commands.UpdateActivityType;

public record UpdateActivityTypeCommand(Guid UserId, int Id, string Name) : IRequest<int>;

public class UpdateActivityTypeCommandValidator : AbstractValidator<UpdateActivityTypeCommand>
{
    public UpdateActivityTypeCommandValidator()
    {
    }
}

public class UpdateActivityTypeCommandHandler : IRequestHandler<UpdateActivityTypeCommand, int>
{
    private readonly IApplicationDbContext _context;
    private readonly IActivityTypeRepository _activityTypeRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateActivityTypeCommandHandler(IApplicationDbContext context,
        IActivityTypeRepository activityTypeRepository,
        IUnitOfWork unitOfWork)
    {
        _context = context;
        _activityTypeRepository = activityTypeRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<int> Handle(UpdateActivityTypeCommand request, CancellationToken cancellationToken)
    {
        ActivityType? at = await _activityTypeRepository.GetByIdAsync(request.UserId, request.Id, cancellationToken);
        Guard.Against.NotFound(request.Id, at);

        at.Rename(request.Name);

        return await _unitOfWork.SaveChangesAsync();
    }
}
