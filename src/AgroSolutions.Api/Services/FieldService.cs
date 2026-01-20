using AgroSolutions.Api.Models;
using AgroSolutions.Domain.Entities;
using AgroSolutions.Domain.Exceptions;
using AgroSolutions.Domain.Repositories;
using AgroSolutions.Domain.ValueObjects;

namespace AgroSolutions.Api.Services;

/// <summary>
/// Service implementation for Field management
/// </summary>
public class FieldService : IFieldService
{
    private readonly IFieldRepository _fieldRepository;
    private readonly IFarmRepository _farmRepository;
    private readonly ILogger<FieldService> _logger;

    public FieldService(
        IFieldRepository fieldRepository,
        IFarmRepository farmRepository,
        ILogger<FieldService> logger)
    {
        _fieldRepository = fieldRepository;
        _farmRepository = farmRepository;
        _logger = logger;
    }

    public async Task<IEnumerable<FieldDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var fields = await _fieldRepository.GetAllAsync(cancellationToken);
        return fields.Select(MapToDto);
    }

    public async Task<IEnumerable<FieldDto>> GetByFarmIdAsync(Guid farmId, CancellationToken cancellationToken = default)
    {
        var fields = await _fieldRepository.GetByFarmIdAsync(farmId, cancellationToken);
        return fields.Select(MapToDto);
    }

    public async Task<FieldDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var field = await _fieldRepository.GetByIdAsync(id, cancellationToken);
        return field == null ? null : MapToDto(field);
    }

    public async Task<FieldDto> CreateAsync(Guid farmId, CreateFieldDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            // Verify farm exists
            var farm = await _farmRepository.GetByIdAsync(farmId, cancellationToken);
            if (farm == null)
                throw new DomainException($"Farm with ID {farmId} not found");

            var property = new Property(
                dto.Property.Name,
                dto.Property.Location,
                dto.Property.Area,
                dto.Property.Description
            );

            var field = new Field(
                farmId,
                property,
                dto.CropType,
                dto.PlantingDate,
                dto.HarvestDate
            );

            await _fieldRepository.AddAsync(field, cancellationToken);
            await _fieldRepository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Created field {FieldId} for farm {FarmId}", field.Id, farmId);

            return MapToDto(field);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating field for farm {FarmId}", farmId);
            throw new DomainException($"Failed to create field: {ex.Message}", ex);
        }
    }

    public async Task<FieldDto?> UpdateAsync(Guid id, UpdateFieldDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            var field = await _fieldRepository.GetByIdAsync(id, cancellationToken);
            if (field == null)
                return null;

            if (dto.Property != null)
            {
                var property = new Property(
                    dto.Property.Name,
                    dto.Property.Location,
                    dto.Property.Area,
                    dto.Property.Description
                );
                field.UpdateProperty(property);
            }

            if (!string.IsNullOrWhiteSpace(dto.CropType))
            {
                field.UpdateCropInfo(
                    dto.CropType,
                    dto.PlantingDate,
                    dto.HarvestDate
                );
            }

            await _fieldRepository.UpdateAsync(field, cancellationToken);
            await _fieldRepository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Updated field {FieldId}", field.Id);

            return MapToDto(field);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating field {FieldId}", id);
            throw new DomainException($"Failed to update field: {ex.Message}", ex);
        }
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var deleted = await _fieldRepository.DeleteAsync(id, cancellationToken);
            if (deleted)
            {
                await _fieldRepository.SaveChangesAsync(cancellationToken);
                _logger.LogInformation("Deleted field {FieldId}", id);
            }
            return deleted;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting field {FieldId}", id);
            throw new DomainException($"Failed to delete field: {ex.Message}", ex);
        }
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _fieldRepository.ExistsAsync(id, cancellationToken);
    }

    private static FieldDto MapToDto(Field field)
    {
        return new FieldDto
        {
            Id = field.Id,
            FarmId = field.FarmId,
            Property = new PropertyDto
            {
                Name = field.Property.Name,
                Location = field.Property.Location,
                Area = field.Property.Area,
                Description = field.Property.Description
            },
            CropType = field.CropType,
            PlantingDate = field.PlantingDate,
            HarvestDate = field.HarvestDate,
            CreatedAt = field.CreatedAt,
            UpdatedAt = field.UpdatedAt
        };
    }
}
