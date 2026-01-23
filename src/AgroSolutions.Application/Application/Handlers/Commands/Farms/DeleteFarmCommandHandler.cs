using AgroSolutions.Application.Commands.Farms;
using Microsoft.Extensions.Logging;
using AgroSolutions.Application.Common.Notifications;
using AgroSolutions.Application.Common.Results;
using AgroSolutions.Domain.Repositories;
using MediatR;

namespace AgroSolutions.Application.Handlers.Commands.Farms;

/// <summary>
/// Handler for DeleteFarmCommand
/// </summary>
public class DeleteFarmCommandHandler : IRequestHandler<DeleteFarmCommand, Result>
{
    private readonly IFarmRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeleteFarmCommandHandler> _logger;
    private readonly NotificationContext _notificationContext;

    public DeleteFarmCommandHandler(
        IFarmRepository repository,
        IUnitOfWork unitOfWork,
        ILogger<DeleteFarmCommandHandler> logger,
        NotificationContext notificationContext)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _logger = logger;
        _notificationContext = notificationContext;
    }

    public async Task<Result> Handle(DeleteFarmCommand request, CancellationToken cancellationToken)
    {
        var farm = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (farm == null)
        {
            _notificationContext.AddNotification("Farm", $"Farm with ID {request.Id} not found");
            return Result.Failure(_notificationContext.Notifications);
        }

        var deleted = await _repository.DeleteAsync(request.Id, cancellationToken);
        if (deleted)
        {
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Deleted farm {FarmId}", request.Id);
            return Result.Success();
        }

        _notificationContext.AddNotification("Farm", $"Failed to delete farm with ID {request.Id}");
        return Result.Failure(_notificationContext.Notifications);
    }
}
