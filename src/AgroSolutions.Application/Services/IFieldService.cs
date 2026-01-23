using AgroSolutions.Application.Common.Results;
using AgroSolutions.Application.Models;

namespace AgroSolutions.Application.Services;

/// <summary>
/// Service interface for Field management
/// </summary>
public interface IFieldService
{
    Task<IEnumerable<FieldDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<FieldDto>> GetByFarmIdAsync(Guid farmId, CancellationToken cancellationToken = default);
    Task<FieldDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<FieldDto>> CreateFieldAsync(Guid farmId, CreateFieldDto dto, CancellationToken cancellationToken = default);
    Task<Result<FieldDto>> UpdateFieldAsync(Guid id, UpdateFieldDto dto, CancellationToken cancellationToken = default);
    Task<Result> DeleteFieldAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
}
