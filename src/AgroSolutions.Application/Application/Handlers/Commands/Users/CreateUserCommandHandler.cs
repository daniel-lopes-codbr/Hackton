using AgroSolutions.Application.Commands.Users;
using Microsoft.Extensions.Logging;
using AgroSolutions.Application.Common.Notifications;
using AgroSolutions.Application.Common.Results;
using AgroSolutions.Domain.Entities;
using AgroSolutions.Domain.Repositories;
using AutoMapper;
using MediatR;

namespace AgroSolutions.Application.Handlers.Commands.Users;

/// <summary>
/// Handler for CreateUserCommand
/// </summary>
public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Result<Models.UserDto>>
{
    private readonly IUserRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<CreateUserCommandHandler> _logger;
    private readonly NotificationContext _notificationContext;

    public CreateUserCommandHandler(
        IUserRepository repository,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<CreateUserCommandHandler> logger,
        NotificationContext notificationContext)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
        _notificationContext = notificationContext;
    }

    public async Task<Result<Models.UserDto>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        // Validação de negócio com Notification Pattern
        if (await _repository.ExistsByEmailAsync(request.Email, cancellationToken))
        {
            _notificationContext.AddNotification("Email", $"User with email {request.Email} already exists");
            return Result<Models.UserDto>.Failure(_notificationContext.Notifications);
        }

        // Criar entidade
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
        var user = new User(request.Name, request.Email, passwordHash, request.Role);

        // Salvar
        await _repository.AddAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Mapear Entity → DTO usando AutoMapper
        var userDto = _mapper.Map<Models.UserDto>(user);

        _logger.LogInformation("Created user {UserId} with email {Email} and role {Role}", user.Id, user.Email, user.Role);

        return Result<Models.UserDto>.Success(userDto);
    }
}
