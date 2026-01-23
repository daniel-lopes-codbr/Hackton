using AgroSolutions.Application.Queries.Users;
using AgroSolutions.Application.Models;
using AgroSolutions.Domain.Repositories;
using AutoMapper;
using MediatR;

namespace AgroSolutions.Application.Handlers.Queries.Users;

/// <summary>
/// Handler for GetAllUsersQuery
/// </summary>
public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, IEnumerable<UserDto>>
{
    private readonly IUserRepository _repository;
    private readonly IMapper _mapper;

    public GetAllUsersQueryHandler(IUserRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<UserDto>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        var users = await _repository.GetAllAsync(cancellationToken);
        return users.Select(u => _mapper.Map<UserDto>(u));
    }
}
