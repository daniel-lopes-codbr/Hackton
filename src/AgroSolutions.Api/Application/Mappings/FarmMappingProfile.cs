using AgroSolutions.Api.Application.Commands.Farms;
using AgroSolutions.Api.Models;
using AgroSolutions.Domain.Entities;
using AgroSolutions.Domain.ValueObjects;
using AutoMapper;

namespace AgroSolutions.Api.Application.Mappings;

/// <summary>
/// AutoMapper profile for Farm mappings
/// </summary>
public class FarmMappingProfile : Profile
{
    public FarmMappingProfile()
    {
        // DTO → Command (usado no Service)
        CreateMap<CreateFarmDto, CreateFarmCommand>();
        CreateMap<UpdateFarmDto, UpdateFarmCommand>()
            .ForMember(dest => dest.Id, opt => opt.Ignore()); // Id vem do route parameter
        
        // Entity → DTO (usado no Handler)
        CreateMap<Farm, FarmDto>()
            .ForMember(dest => dest.Property, opt => opt.MapFrom(src => new PropertyDto
            {
                Name = src.Property.Name,
                Location = src.Property.Location,
                Area = src.Property.Area,
                Description = src.Property.Description
            }));
        
        // Value Object → DTO
        CreateMap<Property, PropertyDto>();
    }
}
