using AgroSolutions.Domain.Entities;
using AgroSolutions.Domain.Repositories;
using AgroSolutions.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AgroSolutions.Infrastructure.Data.Repositories;

/// <summary>
/// Repository implementation for Alert entity
/// </summary>
public class AlertRepository : IAlertRepository
{
    private readonly AgroSolutionsDbContext _context;

    public AlertRepository(AgroSolutionsDbContext context)
    {
        _context = context;
    }

    public async Task<Alert?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Alerts
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Alert>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Alerts
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Alert>> GetByFieldIdAsync(Guid fieldId, CancellationToken cancellationToken = default)
    {
        return await _context.Alerts
            .Where(a => a.FieldId == fieldId)
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    // If needed, alerts by farm can be obtained by joining fields -> farms in a custom query/service.

    public async Task<IEnumerable<Alert>> GetActiveAlertsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Alerts
            .Where(a => a.IsEnable)
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Alert>> GetAlertsCreatedBeforeAsync(DateTime date, CancellationToken cancellationToken = default)
    {
        return await _context.Alerts
            .Where(a => a.CreatedAt < date && a.IsEnable)
            .ToListAsync(cancellationToken);
    }

    public async Task<Alert> AddAsync(Alert alert, CancellationToken cancellationToken = default)
    {
        await _context.Alerts.AddAsync(alert, cancellationToken);
        return alert;
    }

    public async Task<IEnumerable<Alert>> AddRangeAsync(IEnumerable<Alert> alerts, CancellationToken cancellationToken = default)
    {
        var alertsList = alerts.ToList();
        await _context.Alerts.AddRangeAsync(alertsList, cancellationToken);
        return alertsList;
    }

    public Task UpdateAsync(Alert alert, CancellationToken cancellationToken = default)
    {
        _context.Alerts.Update(alert);
        return Task.CompletedTask;
    }

    public Task UpdateRangeAsync(IEnumerable<Alert> alerts, CancellationToken cancellationToken = default)
    {
        _context.Alerts.UpdateRange(alerts);
        return Task.CompletedTask;
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Alerts
            .AnyAsync(a => a.Id == id, cancellationToken);
    }
}
