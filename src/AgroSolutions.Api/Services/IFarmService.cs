using AgroSolutions.Api.Application.Common.Results;
using AgroSolutions.Api.Models;

namespace AgroSolutions.Api.Services;

/// <summary>
/// Service interface for Farm management
/// </summary>
public interface IFarmService
{
    Task<IEnumerable<FarmDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<FarmDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<FarmDto>> CreateFarmAsync(CreateFarmDto dto, CancellationToken cancellationToken = default);
    Task<Result<FarmDto>> UpdateFarmAsync(Guid id, UpdateFarmDto dto, CancellationToken cancellationToken = default);
    Task<Result> DeleteFarmAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
}
