using AgroSolutions.Domain.Entities;

namespace AgroSolutions.Domain.Repositories;

/// <summary>
/// Repository interface for Farm entity
/// </summary>
public interface IFarmRepository
{
    Task<Farm?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Farm>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Farm>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Farm>> GetByOwnerNameAsync(string ownerName, CancellationToken cancellationToken = default);
    Task<Farm> AddAsync(Farm farm, CancellationToken cancellationToken = default);
    Task<Farm> UpdateAsync(Farm farm, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<int> CountAsync(CancellationToken cancellationToken = default);
}
