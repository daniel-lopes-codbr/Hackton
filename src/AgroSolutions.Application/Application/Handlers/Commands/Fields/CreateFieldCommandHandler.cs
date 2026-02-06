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
/// Handler for CreateFieldCommand
/// </summary>
public class CreateFieldCommandHandler : IRequestHandler<CreateFieldCommand, Result<Models.FieldDto>>
{
    private readonly IFieldRepository _fieldRepository;
    private readonly IFarmRepository _farmRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<CreateFieldCommandHandler> _logger;
    private readonly NotificationContext _notificationContext;

    public CreateFieldCommandHandler(
        IFieldRepository fieldRepository,
        IFarmRepository farmRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<CreateFieldCommandHandler> logger,
        NotificationContext notificationContext)
    {
        _fieldRepository = fieldRepository;
        _farmRepository = farmRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
        _notificationContext = notificationContext;
    }

    public async Task<Result<Models.FieldDto>> Handle(CreateFieldCommand request, CancellationToken cancellationToken)
    {
        // Verify farm exists
        var farm = await _farmRepository.GetByIdAsync(request.FarmId, cancellationToken);
        if (farm == null)
        {
            _notificationContext.AddNotification("Farm", $"Farm with ID {request.FarmId} not found");
            return Result<Models.FieldDto>.Failure(_notificationContext.Notifications);
        }

        // Create Entity
        var field = new Domain.Entities.Field(
            request.FarmId,
            request.Name,
            request.AreaSquareMeters,
            request.CropType
        );

        // Save
        await _fieldRepository.AddAsync(field, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Map Entity → DTO using AutoMapper
        var fieldDto = _mapper.Map<Models.FieldDto>(field);

        _logger.LogInformation("Created field {FieldId} for farm {FarmId}", field.Id, field.FarmId);

        return Result<Models.FieldDto>.Success(fieldDto);
    }
}
