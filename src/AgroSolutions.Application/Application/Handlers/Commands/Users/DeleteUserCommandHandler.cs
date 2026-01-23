using AgroSolutions.Application.Commands.Users;
using Microsoft.Extensions.Logging;
using AgroSolutions.Application.Common.Notifications;
using AgroSolutions.Application.Common.Results;
using AgroSolutions.Domain.Repositories;
using MediatR;

namespace AgroSolutions.Application.Handlers.Commands.Users;

/// <summary>
/// Handler for DeleteUserCommand
/// </summary>
public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, Result>
{
    private readonly IUserRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeleteUserCommandHandler> _logger;
    private readonly NotificationContext _notificationContext;

    public DeleteUserCommandHandler(
        IUserRepository repository,
        IUnitOfWork unitOfWork,
        ILogger<DeleteUserCommandHandler> logger,
        NotificationContext notificationContext)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _logger = logger;
        _notificationContext = notificationContext;
    }

    public async Task<Result> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (user == null)
        {
            _notificationContext.AddNotification("User", $"User with ID {request.Id} not found");
            return Result.Failure(_notificationContext.Notifications);
        }

        var deleted = await _repository.DeleteAsync(request.Id, cancellationToken);
        if (deleted)
        {
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Deleted user {UserId}", request.Id);
            return Result.Success();
        }

        _notificationContext.AddNotification("User", $"Failed to delete user with ID {request.Id}");
        return Result.Failure(_notificationContext.Notifications);
    }
}
