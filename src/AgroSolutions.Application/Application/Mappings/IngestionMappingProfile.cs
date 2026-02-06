using AgroSolutions.Application.Commands.Ingestion;
using AgroSolutions.Application.Models;
using AgroSolutions.Domain.Entities;
using AutoMapper;

namespace AgroSolutions.Application.Mappings;

/// <summary>
/// AutoMapper profile for Ingestion mappings
/// </summary>
public class IngestionMappingProfile : Profile
{
    public IngestionMappingProfile()
    {
        // DTO → Command (usado no Service)
        CreateMap<SensorReadingDto, IngestSingleCommand>()
            .ForMember(dest => dest.Reading, opt => opt.MapFrom(src => src));
        CreateMap<BatchSensorReadingDto, IngestBatchCommand>()
            .ForMember(dest => dest.Batch, opt => opt.MapFrom(src => src));
        CreateMap<BatchSensorReadingDto, IngestBatchParallelCommand>()
            .ForMember(dest => dest.Batch, opt => opt.MapFrom(src => src));
        
        // Entity → DTO (usado no Handler)
        CreateMap<SensorReading, SensorReadingDto>()
            .ForMember(dest => dest.FieldId, opt => opt.MapFrom(src => src.FieldId))
            .ForMember(dest => dest.SoilMoisture, opt => opt.MapFrom(src => src.SoilMoisture))
            .ForMember(dest => dest.AirTemperature, opt => opt.MapFrom(src => src.AirTemperature))
            .ForMember(dest => dest.Precipitation, opt => opt.MapFrom(src => src.Precipitation))
            .ForMember(dest => dest.IsRichInPests, opt => opt.MapFrom(src => src.IsRichInPests))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt));
    }
}
