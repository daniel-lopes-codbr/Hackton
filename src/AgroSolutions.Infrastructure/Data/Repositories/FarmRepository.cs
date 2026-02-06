using AgroSolutions.Domain.Entities;
using AgroSolutions.Domain.Repositories;
using AgroSolutions.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AgroSolutions.Infrastructure.Data.Repositories;

/// <summary>
/// Repository implementation for Farm entity
/// </summary>
public class FarmRepository : IFarmRepository
{
    private readonly AgroSolutionsDbContext _context;

    public FarmRepository(AgroSolutionsDbContext context)
    {
        _context = context;
    }

    public async Task<Farm?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Farms
            .FirstOrDefaultAsync(f => f.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Farm>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Farms
            .OrderBy(f => f.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Farm>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.Farms
            .Where(f => f.UserId == userId)
            .OrderBy(f => f.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Farm>> GetByOwnerNameAsync(string ownerName, CancellationToken cancellationToken = default)
    {
        return await _context.Farms
            .Where(f => f.Name.Contains(ownerName))
            .OrderBy(f => f.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<Farm> AddAsync(Farm farm, CancellationToken cancellationToken = default)
    {
        await _context.Farms.AddAsync(farm, cancellationToken);
        return farm;
    }

    public Task<Farm> UpdateAsync(Farm farm, CancellationToken cancellationToken = default)
    {
        _context.Farms.Update(farm);
        return Task.FromResult(farm);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var farm = await GetByIdAsync(id, cancellationToken);
        if (farm == null)
            return false;

        _context.Farms.Remove(farm);
        return true;
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Farms
            .AnyAsync(f => f.Id == id, cancellationToken);
    }

    public async Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Farms
            .CountAsync(cancellationToken);
    }
}
