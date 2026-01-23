using AgroSolutions.Application.Commands.Users;
using Microsoft.Extensions.Logging;
using AgroSolutions.Application.Common.Notifications;
using AgroSolutions.Application.Common.Results;
using AgroSolutions.Domain.Repositories;
using AutoMapper;
using MediatR;

namespace AgroSolutions.Application.Handlers.Commands.Users;

/// <summary>
/// Handler for UpdateUserCommand
/// </summary>
public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, Result<Models.UserDto>>
{
    private readonly IUserRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<UpdateUserCommandHandler> _logger;
    private readonly NotificationContext _notificationContext;

    public UpdateUserCommandHandler(
        IUserRepository repository,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<UpdateUserCommandHandler> logger,
        NotificationContext notificationContext)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
        _notificationContext = notificationContext;
    }

    public async Task<Result<Models.UserDto>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (user == null)
        {
            _notificationContext.AddNotification("User", $"User with ID {request.Id} not found");
            return Result<Models.UserDto>.Failure(_notificationContext.Notifications);
        }

        // Check email uniqueness if email is being updated
        if (!string.IsNullOrWhiteSpace(request.Email) && request.Email != user.Email)
        {
            if (await _repository.ExistsByEmailAsync(request.Email, cancellationToken))
            {
                _notificationContext.AddNotification("Email", $"User with email {request.Email} already exists");
                return Result<Models.UserDto>.Failure(_notificationContext.Notifications);
            }
        }

        // Update user properties
        if (!string.IsNullOrWhiteSpace(request.Name))
            user.UpdateName(request.Name);

        if (!string.IsNullOrWhiteSpace(request.Email))
            user.UpdateEmail(request.Email);

        if (!string.IsNullOrWhiteSpace(request.Password))
        {
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
            user.UpdatePasswordHash(passwordHash);
        }

        if (!string.IsNullOrWhiteSpace(request.Role))
            user.UpdateRole(request.Role);

        // Save changes
        await _repository.UpdateAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Map Entity → DTO using AutoMapper
        var userDto = _mapper.Map<Models.UserDto>(user);

        _logger.LogInformation("Updated user {UserId}", user.Id);

        return Result<Models.UserDto>.Success(userDto);
    }
}
