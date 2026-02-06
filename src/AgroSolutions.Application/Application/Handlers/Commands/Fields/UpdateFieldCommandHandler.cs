using AgroSolutions.Application.Commands.Fields;
using Microsoft.Extensions.Logging;
using AgroSolutions.Application.Common.Notifications;
using AgroSolutions.Application.Common.Results;
using AgroSolutions.Domain.Repositories;
using AgroSolutions.Domain.ValueObjects;
using AutoMapper;
using MediatR;

namespace AgroSolutions.Application.Handlers.Commands.Fields;

/// <summary>
/// Handler for UpdateFieldCommand
/// </summary>
public class UpdateFieldCommandHandler : IRequestHandler<UpdateFieldCommand, Result<Models.FieldDto>>
{
    private readonly IFieldRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<UpdateFieldCommandHandler> _logger;
    private readonly NotificationContext _notificationContext;

    public UpdateFieldCommandHandler(
        IFieldRepository repository,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<UpdateFieldCommandHandler> logger,
        NotificationContext notificationContext)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
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

        // Update name/area if provided
        if (!string.IsNullOrWhiteSpace(request.Name))
        {
            field.UpdateName(request.Name);
        }

        if (request.AreaSquareMeters.HasValue)
        {
            field.UpdateArea(request.AreaSquareMeters.Value);
        }

        // Update crop info if provided
        if (!string.IsNullOrWhiteSpace(request.CropType))
        {
            field.UpdateCropType(request.CropType);
        }

        // Save changes
        await _repository.UpdateAsync(field, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Map Entity → DTO using AutoMapper
        var fieldDto = _mapper.Map<Models.FieldDto>(field);

        _logger.LogInformation("Updated field {FieldId}", field.Id);

        return Result<Models.FieldDto>.Success(fieldDto);
    }
}
