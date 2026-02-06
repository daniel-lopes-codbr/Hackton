using AgroSolutions.Application.Commands.Users;
using AgroSolutions.Application.Models;
using AgroSolutions.Domain.Entities;
using AutoMapper;

namespace AgroSolutions.Application.Mappings;

/// <summary>
/// AutoMapper profile for User mappings
/// </summary>
public class UserMappingProfile : Profile
{
    public UserMappingProfile()
    {
        // DTO → Command (usado no Service)
        CreateMap<CreateUserDto, CreateUserCommand>();
        CreateMap<UpdateUserDto, UpdateUserCommand>()
            .ForMember(dest => dest.Id, opt => opt.Ignore()); // Id vem do route parameter
        
        // Entity → DTO (usado no Handler)
        CreateMap<User, UserDto>();
        CreateMap<UserAuthorization, UserAuthorizationDto>()
            .ForMember(dest => dest.LastUpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt));
    }
}
