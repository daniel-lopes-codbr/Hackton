using AgroSolutions.Api.Models;
using AgroSolutions.Domain.Entities;
using AgroSolutions.Domain.Exceptions;
using AgroSolutions.Domain.Repositories;
using AgroSolutions.Domain.ValueObjects;

namespace AgroSolutions.Api.Services;

/// <summary>
/// Service implementation for Farm management
/// </summary>
public class FarmService : IFarmService
{
    private readonly IFarmRepository _repository;
    private readonly ILogger<FarmService> _logger;

    public FarmService(IFarmRepository repository, ILogger<FarmService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<IEnumerable<FarmDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var farms = await _repository.GetAllAsync(cancellationToken);
        return farms.Select(MapToDto);
    }

    public async Task<FarmDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var farm = await _repository.GetByIdAsync(id, cancellationToken);
        return farm == null ? null : MapToDto(farm);
    }

    public async Task<FarmDto> CreateAsync(CreateFarmDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            var property = new Property(
                dto.Property.Name,
                dto.Property.Location,
                dto.Property.Area,
                dto.Property.Description
            );

            var farm = new Farm(
                property,
                dto.OwnerName,
                dto.OwnerEmail,
                dto.OwnerPhone
            );

            await _repository.AddAsync(farm, cancellationToken);
            await _repository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Created farm {FarmId} for owner {OwnerName}", farm.Id, farm.OwnerName);

            return MapToDto(farm);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating farm");
            throw new DomainException($"Failed to create farm: {ex.Message}", ex);
        }
    }

    public async Task<FarmDto?> UpdateAsync(Guid id, UpdateFarmDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            var farm = await _repository.GetByIdAsync(id, cancellationToken);
            if (farm == null)
                return null;

            if (dto.Property != null)
            {
                var property = new Property(
                    dto.Property.Name,
                    dto.Property.Location,
                    dto.Property.Area,
                    dto.Property.Description
                );
                farm.UpdateProperty(property);
            }

            if (!string.IsNullOrWhiteSpace(dto.OwnerName))
            {
                farm.UpdateOwnerInfo(
                    dto.OwnerName,
                    dto.OwnerEmail,
                    dto.OwnerPhone
                );
            }

            await _repository.UpdateAsync(farm, cancellationToken);
            await _repository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Updated farm {FarmId}", farm.Id);

            return MapToDto(farm);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating farm {FarmId}", id);
            throw new DomainException($"Failed to update farm: {ex.Message}", ex);
        }
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var deleted = await _repository.DeleteAsync(id, cancellationToken);
            if (deleted)
            {
                await _repository.SaveChangesAsync(cancellationToken);
                _logger.LogInformation("Deleted farm {FarmId}", id);
            }
            return deleted;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting farm {FarmId}", id);
            throw new DomainException($"Failed to delete farm: {ex.Message}", ex);
        }
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _repository.ExistsAsync(id, cancellationToken);
    }

    private static FarmDto MapToDto(Farm farm)
    {
        return new FarmDto
        {
            Id = farm.Id,
            Property = new PropertyDto
            {
                Name = farm.Property.Name,
                Location = farm.Property.Location,
                Area = farm.Property.Area,
                Description = farm.Property.Description
            },
            OwnerName = farm.OwnerName,
            OwnerEmail = farm.OwnerEmail,
            OwnerPhone = farm.OwnerPhone,
            CreatedAt = farm.CreatedAt,
            UpdatedAt = farm.UpdatedAt
        };
    }
}
