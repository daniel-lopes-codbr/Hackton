using AgroSolutions.Api.Models;
using AgroSolutions.Domain.Entities;

namespace AgroSolutions.Api.Services;

/// <summary>
/// Service interface for Field management
/// </summary>
public interface IFieldService
{
    Task<IEnumerable<FieldDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<FieldDto>> GetByFarmIdAsync(Guid farmId, CancellationToken cancellationToken = default);
    Task<FieldDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<FieldDto> CreateAsync(Guid farmId, CreateFieldDto dto, CancellationToken cancellationToken = default);
    Task<FieldDto?> UpdateAsync(Guid id, UpdateFieldDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
}
