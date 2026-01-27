using AgroSolutions.Application.Common.Results;
using AgroSolutions.Application.Models;

namespace AgroSolutions.Application.Services;

/// <summary>
/// Service interface for Alert management
/// </summary>
public interface IAlertService
{
    /// <summary>
    /// Creates alerts based on sensor readings from the last hour
    /// </summary>
    Task<Result<AlertCreationResponseDto>> CreateAlertsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Deactivates alerts created the previous day
    /// </summary>
    Task<Result<int>> UpdateAlertsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all alerts
    /// </summary>
    Task<IEnumerable<AlertDto>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets an alert by ID
    /// </summary>
    Task<AlertDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets alerts by FieldId
    /// </summary>
    Task<IEnumerable<AlertDto>> GetByFieldIdAsync(Guid fieldId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets alerts by FarmId
    /// </summary>
    Task<IEnumerable<AlertDto>> GetByFarmIdAsync(Guid farmId, CancellationToken cancellationToken = default);
}
