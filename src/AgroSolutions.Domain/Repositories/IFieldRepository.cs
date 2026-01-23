using AgroSolutions.Domain.Entities;

namespace AgroSolutions.Domain.Repositories;

/// <summary>
/// Repository interface for Field entity
/// </summary>
public interface IFieldRepository
{
    Task<Field?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Field>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Field>> GetByFarmIdAsync(Guid farmId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Field>> GetByCropTypeAsync(string cropType, CancellationToken cancellationToken = default);
    Task<Field> AddAsync(Field field, CancellationToken cancellationToken = default);
    Task<Field> UpdateAsync(Field field, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> ExistsByFarmIdAsync(Guid farmId, CancellationToken cancellationToken = default);
    Task<int> CountByFarmIdAsync(Guid farmId, CancellationToken cancellationToken = default);
}
