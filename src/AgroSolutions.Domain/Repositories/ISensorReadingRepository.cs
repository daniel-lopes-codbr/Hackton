using AgroSolutions.Domain.Entities;

namespace AgroSolutions.Domain.Repositories;

/// <summary>
/// Repository interface for SensorReading entity
/// </summary>
public interface ISensorReadingRepository
{
    Task<SensorReading?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<SensorReading>> GetByFieldIdAsync(Guid fieldId, CancellationToken cancellationToken = default);
    Task<IEnumerable<SensorReading>> GetByFieldIdAndSensorTypeAsync(Guid fieldId, string sensorType, CancellationToken cancellationToken = default);
    Task<IEnumerable<SensorReading>> GetByTimestampRangeAsync(DateTime startTime, DateTime endTime, CancellationToken cancellationToken = default);
    Task<SensorReading> AddAsync(SensorReading reading, CancellationToken cancellationToken = default);
    Task<IEnumerable<SensorReading>> AddRangeAsync(IEnumerable<SensorReading> readings, CancellationToken cancellationToken = default);
    Task<int> CountByFieldIdAsync(Guid fieldId, CancellationToken cancellationToken = default);
}
