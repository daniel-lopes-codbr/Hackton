using AgroSolutions.Application.Models;
using AgroSolutions.Domain.Entities;
using AutoMapper;

namespace AgroSolutions.Application.Mappings;

/// <summary>
/// AutoMapper profile for Alert mappings
/// </summary>
public class AlertMappingProfile : Profile
{
    public AlertMappingProfile()
    {
        // Entity → DTO (used in Handlers)
        CreateMap<Alert, AlertDto>()
            .ForMember(dest => dest.FieldId, opt => opt.MapFrom(src => src.FieldId))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
            .ForMember(dest => dest.IsEnable, opt => opt.MapFrom(src => src.IsEnable))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt));
    }
}
