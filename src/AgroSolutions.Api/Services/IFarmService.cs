using AgroSolutions.Api.Models;
using AgroSolutions.Domain.Entities;

namespace AgroSolutions.Api.Services;

/// <summary>
/// Service interface for Farm management
/// </summary>
public interface IFarmService
{
    Task<IEnumerable<FarmDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<FarmDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<FarmDto> CreateAsync(CreateFarmDto dto, CancellationToken cancellationToken = default);
    Task<FarmDto?> UpdateAsync(Guid id, UpdateFarmDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
}
