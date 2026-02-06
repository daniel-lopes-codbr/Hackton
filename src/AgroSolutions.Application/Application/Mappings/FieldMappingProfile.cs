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
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.AreaSquareMeters, opt => opt.MapFrom(src => src.AreaSquareMeters))
            .ForMember(dest => dest.CropType, opt => opt.MapFrom(src => src.CropType))
            .ForMember(dest => dest.FarmId, opt => opt.MapFrom(src => src.FarmId))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
            .ForMember(dest => dest.LastUpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt));
    }
}
