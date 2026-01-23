using AgroSolutions.Application.Commands.Fields;
using Microsoft.Extensions.Logging;
using AgroSolutions.Application.Common.Notifications;
using AgroSolutions.Application.Common.Results;
using AgroSolutions.Domain.Repositories;
using MediatR;

namespace AgroSolutions.Application.Handlers.Commands.Fields;

/// <summary>
/// Handler for DeleteFieldCommand
/// </summary>
public class DeleteFieldCommandHandler : IRequestHandler<DeleteFieldCommand, Result>
{
    private readonly IFieldRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeleteFieldCommandHandler> _logger;
    private readonly NotificationContext _notificationContext;

    public DeleteFieldCommandHandler(
        IFieldRepository repository,
        IUnitOfWork unitOfWork,
        ILogger<DeleteFieldCommandHandler> logger,
        NotificationContext notificationContext)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _logger = logger;
        _notificationContext = notificationContext;
    }

    public async Task<Result> Handle(DeleteFieldCommand request, CancellationToken cancellationToken)
    {
        var field = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (field == null)
        {
            _notificationContext.AddNotification("Field", $"Field with ID {request.Id} not found");
            return Result.Failure(_notificationContext.Notifications);
        }

        var deleted = await _repository.DeleteAsync(request.Id, cancellationToken);
        if (deleted)
        {
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Deleted field {FieldId}", request.Id);
            return Result.Success();
        }

        _notificationContext.AddNotification("Field", $"Failed to delete field with ID {request.Id}");
        return Result.Failure(_notificationContext.Notifications);
    }
}
