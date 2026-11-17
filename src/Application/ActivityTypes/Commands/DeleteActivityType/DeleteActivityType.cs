using DailyTracker.Application.Common.Interfaces;
using DailyTracker.Domain.Entities;
using DailyTracker.Domain.Repositories;
using static DailyTracker.Domain.Entities.ActivityType;

namespace DailyTracker.Application.ActivityTypes.Commands.DeleteActivityType;

public record DeleteActivityTypeCommand(Guid UserId, int Id) : IRequest<int>;

public class DeleteActivityTypeCommandValidator : AbstractValidator<DeleteActivityTypeCommand>
{
    public DeleteActivityTypeCommandValidator()
    {
    }
}

public class DeleteActivityTypeCommandHandler : IRequestHandler<DeleteActivityTypeCommand, int>
{
    private readonly IApplicationDbContext _context;
    private readonly IActivityTypeRepository _activityTypeRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteActivityTypeCommandHandler(IApplicationDbContext context,
        IActivityTypeRepository activityTypeRepository,
        IUnitOfWork unitOfWork)
    {
        _context = context;
        _activityTypeRepository = activityTypeRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<int> Handle(DeleteActivityTypeCommand request, CancellationToken cancellationToken)
    {
        ActivityType? at = await _activityTypeRepository.GetByIdAsync(request.UserId, request.Id, cancellationToken);
        Guard.Against.NotFound(request.Id, at);

        ActivityTypeDeleteToken deleteToken = at.Delete();

        _activityTypeRepository.Remove(deleteToken);

        return await _unitOfWork.SaveChangesAsync();
    }
}
