using DailyTracker.Application.Common.Interfaces;
using DailyTracker.Domain.Entities.Activities;
using DailyTracker.Domain.Repositories;

namespace DailyTracker.Application.Activities.Commands.DeleteActivity;

public record DeleteActivityCommand(Guid UserId, int Id) : IRequest<int>;

public class DeleteActivityCommandValidator : AbstractValidator<DeleteActivityCommand>
{
    public DeleteActivityCommandValidator()
    {
    }
}

public class DeleteActivityCommandHandler : IRequestHandler<DeleteActivityCommand, int>
{
    private readonly IApplicationDbContext _context;
    private readonly IActivityRepository _activityRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteActivityCommandHandler(IApplicationDbContext context,
        IActivityRepository activityRepository,
        IUnitOfWork unitOfWork)
    {
        _context = context;
        _activityRepository = activityRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<int> Handle(DeleteActivityCommand request, CancellationToken cancellationToken)
    {
        Activity? a = await _activityRepository.GetByIdAsync(request.UserId, request.Id, cancellationToken);
        Guard.Against.NotFound(request.Id, a);

        _activityRepository.Remove(a);

        return await _unitOfWork.SaveChangesAsync();
    }
}
