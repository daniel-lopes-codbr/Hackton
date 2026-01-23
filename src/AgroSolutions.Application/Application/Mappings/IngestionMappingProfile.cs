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
        CreateMap<SensorReading, SensorReadingDto>();
    }
}
