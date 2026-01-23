using AgroSolutions.Api.Application.Commands.Users;
using AgroSolutions.Api.Application.Common.Notifications;
using AgroSolutions.Api.Application.Common.Results;
using AgroSolutions.Domain.Repositories;
using MediatR;

namespace AgroSolutions.Api.Application.Handlers.Commands.Users;

/// <summary>
/// Handler for DeleteUserCommand
/// </summary>
public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, Result>
{
    private readonly IUserRepository _repository;
    private readonly ILogger<DeleteUserCommandHandler> _logger;
    private readonly NotificationContext _notificationContext;

    public DeleteUserCommandHandler(
        IUserRepository repository,
        ILogger<DeleteUserCommandHandler> logger,
        NotificationContext notificationContext)
    {
        _repository = repository;
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
            await _repository.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Deleted user {UserId}", request.Id);
            return Result.Success();
        }

        _notificationContext.AddNotification("User", $"Failed to delete user with ID {request.Id}");
        return Result.Failure(_notificationContext.Notifications);
    }
}
