using AgroSolutions.Application.Commands.Fields;
using AgroSolutions.Application.Models;
using AgroSolutions.Domain.Entities;
using AgroSolutions.Domain.ValueObjects;
using AutoMapper;

namespace AgroSolutions.Application.Mappings;

/// <summary>
/// AutoMapper profile for Field mappings
/// </summary>
public class FieldMappingProfile : Profile
{
    public FieldMappingProfile()
    {
        // DTO → Command (usado no Service)
        CreateMap<CreateFieldDto, CreateFieldCommand>()
            .ForMember(dest => dest.FarmId, opt => opt.Ignore()); // FarmId vem do route parameter
        CreateMap<UpdateFieldDto, UpdateFieldCommand>()
            .ForMember(dest => dest.Id, opt => opt.Ignore()); // Id vem do route parameter
        
        // Entity → DTO (usado no Handler)
        CreateMap<Field, FieldDto>()
            .ForMember(dest => dest.Property, opt => opt.MapFrom(src => new PropertyDto
            {
                Name = src.Property.Name,
                Location = src.Property.Location,
                Area = src.Property.Area,
                Description = src.Property.Description
            }));
    }
}
