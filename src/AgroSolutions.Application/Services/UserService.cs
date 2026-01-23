using AgroSolutions.Application.Commands.Users;
using Microsoft.Extensions.Logging;
using AgroSolutions.Application.Common.Results;
using AgroSolutions.Application.Queries.Users;
using AgroSolutions.Application.Models;
using AgroSolutions.Domain.Repositories;
using AutoMapper;
using MediatR;

namespace AgroSolutions.Application.Services;

/// <summary>
/// Service implementation for User management (uses MediatR internally)
/// </summary>
public class UserService : IUserService
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;
    private readonly IUserRepository _repository; // Keep for simple queries that don't need Commands
    private readonly ILogger<UserService> _logger;

    public UserService(
        IMediator mediator,
        IMapper mapper,
        IUserRepository repository,
        ILogger<UserService> logger)
    {
        _mediator = mediator;
        _mapper = mapper;
        _repository = repository;
        _logger = logger;
    }

    public async Task<IEnumerable<UserDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        // Use Query via MediatR
        var query = new GetAllUsersQuery();
        return await _mediator.Send(query, cancellationToken);
    }

    public async Task<UserDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        // Use Query via MediatR
        var query = new GetUserByIdQuery { Id = id };
        return await _mediator.Send(query, cancellationToken);
    }

    public async Task<UserDto?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        // Simple query - can use repository directly
        var user = await _repository.GetByEmailAsync(email, cancellationToken);
        return user == null ? null : _mapper.Map<UserDto>(user);
    }

    public async Task<Result<UserDto>> CreateUserAsync(CreateUserDto dto, CancellationToken cancellationToken = default)
    {
        // Map DTO → Command using AutoMapper
        var command = _mapper.Map<CreateUserCommand>(dto);
        
        // Send Command via MediatR
        return await _mediator.Send(command, cancellationToken);
    }

    public async Task<Result<UserDto>> UpdateUserAsync(Guid id, UpdateUserDto dto, CancellationToken cancellationToken = default)
    {
        // Map DTO → Command using AutoMapper
        var command = _mapper.Map<UpdateUserCommand>(dto);
        command.Id = id; // Set ID from route parameter
        
        // Send Command via MediatR
        return await _mediator.Send(command, cancellationToken);
    }

    public async Task<Result> DeleteUserAsync(Guid id, CancellationToken cancellationToken = default)
    {
        // Create Command
        var command = new DeleteUserCommand { Id = id };
        
        // Send Command via MediatR
        return await _mediator.Send(command, cancellationToken);
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _repository.ExistsAsync(id, cancellationToken);
    }

    public async Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _repository.ExistsByEmailAsync(email, cancellationToken);
    }
}
