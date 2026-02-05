using AgroSolutions.Application.Common.Results;
using AgroSolutions.Application.Models;

namespace AgroSolutions.Application.Services;

/// <summary>
/// Service interface for Farm management
/// </summary>
public interface IFarmService
{
    Task<IEnumerable<FarmDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<FarmDto>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<FarmDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<FarmDto>> CreateFarmAsync(CreateFarmDto dto, CancellationToken cancellationToken = default);
    Task<Result<FarmDto>> UpdateFarmAsync(Guid id, UpdateFarmDto dto, CancellationToken cancellationToken = default);
    Task<Result> DeleteFarmAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
}
