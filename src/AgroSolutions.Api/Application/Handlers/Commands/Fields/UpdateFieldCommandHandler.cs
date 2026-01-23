using AgroSolutions.Api.Application.Commands.Fields;
using AgroSolutions.Api.Application.Common.Notifications;
using AgroSolutions.Api.Application.Common.Results;
using AgroSolutions.Domain.Repositories;
using AgroSolutions.Domain.ValueObjects;
using AutoMapper;
using MediatR;

namespace AgroSolutions.Api.Application.Handlers.Commands.Fields;

/// <summary>
/// Handler for UpdateFieldCommand
/// </summary>
public class UpdateFieldCommandHandler : IRequestHandler<UpdateFieldCommand, Result<Models.FieldDto>>
{
    private readonly IFieldRepository _repository;
    private readonly IMapper _mapper;
    private readonly ILogger<UpdateFieldCommandHandler> _logger;
    private readonly NotificationContext _notificationContext;

    public UpdateFieldCommandHandler(
        IFieldRepository repository,
        IMapper mapper,
        ILogger<UpdateFieldCommandHandler> logger,
        NotificationContext notificationContext)
    {
        _repository = repository;
        _mapper = mapper;
        _logger = logger;
        _notificationContext = notificationContext;
    }

    public async Task<Result<Models.FieldDto>> Handle(UpdateFieldCommand request, CancellationToken cancellationToken)
    {
        var field = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (field == null)
        {
            _notificationContext.AddNotification("Field", $"Field with ID {request.Id} not found");
            return Result<Models.FieldDto>.Failure(_notificationContext.Notifications);
        }

        // Update property if provided
        if (request.Property != null)
        {
            var property = new Property(
                request.Property.Name,
                request.Property.Location,
                request.Property.Area,
                request.Property.Description
            );
            field.UpdateProperty(property);
        }

        // Update crop info if provided
        if (!string.IsNullOrWhiteSpace(request.CropType))
        {
            field.UpdateCropInfo(
                request.CropType,
                request.PlantingDate,
                request.HarvestDate
            );
        }

        // Save changes
        await _repository.UpdateAsync(field, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        // Map Entity → DTO using AutoMapper
        var fieldDto = _mapper.Map<Models.FieldDto>(field);

        _logger.LogInformation("Updated field {FieldId}", field.Id);

        return Result<Models.FieldDto>.Success(fieldDto);
    }
}
