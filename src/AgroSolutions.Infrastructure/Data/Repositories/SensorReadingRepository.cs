using AgroSolutions.Domain.Entities;
using AgroSolutions.Domain.Repositories;
using AgroSolutions.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AgroSolutions.Infrastructure.Data.Repositories;

/// <summary>
/// Repository implementation for SensorReading entity
/// </summary>
public class SensorReadingRepository : ISensorReadingRepository
{
    private readonly AgroSolutionsDbContext _context;

    public SensorReadingRepository(AgroSolutionsDbContext context)
    {
        _context = context;
    }

    public async Task<SensorReading?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.SensorReadings
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<SensorReading>> GetByFieldIdAsync(Guid fieldId, CancellationToken cancellationToken = default)
    {
        return await _context.SensorReadings
            .Where(r => r.FieldId == fieldId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<SensorReading>> GetByFieldIdAndSensorTypeAsync(Guid fieldId, string sensorType, CancellationToken cancellationToken = default)
    {
        return await _context.SensorReadings
            .Where(r => r.FieldId == fieldId && r.SensorType == sensorType)
            .OrderBy(r => r.ReadingTimestamp)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<SensorReading>> GetByTimestampRangeAsync(DateTime startTime, DateTime endTime, CancellationToken cancellationToken = default)
    {
        return await _context.SensorReadings
            .Where(r => r.CreatedAt >= startTime && r.CreatedAt <= endTime)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<SensorReading> AddAsync(SensorReading reading, CancellationToken cancellationToken = default)
    {
        await _context.SensorReadings.AddAsync(reading, cancellationToken);
        return reading;
    }

    public async Task<IEnumerable<SensorReading>> AddRangeAsync(IEnumerable<SensorReading> readings, CancellationToken cancellationToken = default)
    {
        var readingsList = readings.ToList();
        await _context.SensorReadings.AddRangeAsync(readingsList, cancellationToken);
        return readingsList;
    }

    public async Task<int> CountByFieldIdAsync(Guid fieldId, CancellationToken cancellationToken = default)
    {
        return await _context.SensorReadings
            .CountAsync(r => r.FieldId == fieldId, cancellationToken);
    }
}
