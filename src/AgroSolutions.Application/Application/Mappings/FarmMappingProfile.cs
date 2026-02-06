using AgroSolutions.Application.Commands.Farms;
using AgroSolutions.Application.Models;
using AgroSolutions.Domain.Entities;
using AgroSolutions.Domain.ValueObjects;
using AutoMapper;

namespace AgroSolutions.Application.Mappings;

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
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
            .ForMember(dest => dest.WidthMeters, opt => opt.MapFrom(src => src.WidthMeters))
            .ForMember(dest => dest.LengthMeters, opt => opt.MapFrom(src => src.LengthMeters))
            .ForMember(dest => dest.TotalAreaSquareMeters, opt => opt.MapFrom(src => src.TotalAreaSquareMeters))
            .ForMember(dest => dest.Precipitation, opt => opt.MapFrom(src => src.Precipitation))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
            .ForMember(dest => dest.LastUpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt));

        CreateMap<CreateFarmDto, CreateFarmCommand>();
        CreateMap<UpdateFarmDto, UpdateFarmCommand>().ForMember(dest => dest.Id, opt => opt.Ignore());
    }
}
