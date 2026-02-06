using AgroSolutions.Domain.Entities;

namespace AgroSolutions.Domain.Repositories;

/// <summary>
/// Repository interface for Alert entity
/// </summary>
public interface IAlertRepository
{
    Task<Alert?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Alert>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Alert>> GetByFieldIdAsync(Guid fieldId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Alert>> GetActiveAlertsAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Alert>> GetAlertsCreatedBeforeAsync(DateTime date, CancellationToken cancellationToken = default);
    Task<Alert> AddAsync(Alert alert, CancellationToken cancellationToken = default);
    Task<IEnumerable<Alert>> AddRangeAsync(IEnumerable<Alert> alerts, CancellationToken cancellationToken = default);
    Task UpdateAsync(Alert alert, CancellationToken cancellationToken = default);
    Task UpdateRangeAsync(IEnumerable<Alert> alerts, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
}
