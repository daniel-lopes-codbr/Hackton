using AgroSolutions.Domain.Entities;
using AgroSolutions.Domain.Repositories;
using AgroSolutions.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AgroSolutions.Infrastructure.Data.Repositories;

/// <summary>
/// Repository implementation for Field entity
/// </summary>
public class FieldRepository : IFieldRepository
{
    private readonly AgroSolutionsDbContext _context;

    public FieldRepository(AgroSolutionsDbContext context)
    {
        _context = context;
    }

    public async Task<Field?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Fields
            .FirstOrDefaultAsync(f => f.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Field>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Fields
            .OrderBy(f => f.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Field>> GetByFarmIdAsync(Guid farmId, CancellationToken cancellationToken = default)
    {
        return await _context.Fields
            .Where(f => f.FarmId == farmId)
            .OrderBy(f => f.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Field>> GetByCropTypeAsync(string cropType, CancellationToken cancellationToken = default)
    {
        return await _context.Fields
            .Where(f => f.CropType == cropType)
            .OrderBy(f => f.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<Field> AddAsync(Field field, CancellationToken cancellationToken = default)
    {
        await _context.Fields.AddAsync(field, cancellationToken);
        return field;
    }

    public Task<Field> UpdateAsync(Field field, CancellationToken cancellationToken = default)
    {
        _context.Fields.Update(field);
        return Task.FromResult(field);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var field = await GetByIdAsync(id, cancellationToken);
        if (field == null)
            return false;

        _context.Fields.Remove(field);
        return true;
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Fields
            .AnyAsync(f => f.Id == id, cancellationToken);
    }

    public async Task<bool> ExistsByFarmIdAsync(Guid farmId, CancellationToken cancellationToken = default)
    {
        return await _context.Fields
            .AnyAsync(f => f.FarmId == farmId, cancellationToken);
    }

    public async Task<int> CountByFarmIdAsync(Guid farmId, CancellationToken cancellationToken = default)
    {
        return await _context.Fields
            .CountAsync(f => f.FarmId == farmId, cancellationToken);
    }
}
