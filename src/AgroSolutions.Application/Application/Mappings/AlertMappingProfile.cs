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
        CreateMap<Alert, AlertDto>();
    }
}
