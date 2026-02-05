using AgroSolutions.Application.Commands.Farms;
using Microsoft.Extensions.Logging;
using AgroSolutions.Application.Common.Notifications;
using AgroSolutions.Application.Common.Results;
using AgroSolutions.Domain.Repositories;
using AgroSolutions.Domain.ValueObjects;
using AutoMapper;
using MediatR;

namespace AgroSolutions.Application.Handlers.Commands.Farms;

/// <summary>
/// Handler for UpdateFarmCommand
/// </summary>
public class UpdateFarmCommandHandler : IRequestHandler<UpdateFarmCommand, Result<Models.FarmDto>>
{
    private readonly IFarmRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<UpdateFarmCommandHandler> _logger;
    private readonly NotificationContext _notificationContext;

    public UpdateFarmCommandHandler(
        IFarmRepository repository,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<UpdateFarmCommandHandler> logger,
        NotificationContext notificationContext)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
        _notificationContext = notificationContext;
    }

    public async Task<Result<Models.FarmDto>> Handle(UpdateFarmCommand request, CancellationToken cancellationToken)
    {
        var farm = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (farm == null)
        {
            _notificationContext.AddNotification("Farm", $"Farm with ID {request.Id} not found");
            return Result<Models.FarmDto>.Failure(_notificationContext.Notifications);
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
            farm.UpdateProperty(property);
        }

        // Update owner info if provided
        if (!string.IsNullOrWhiteSpace(request.OwnerName))
        {
            farm.UpdateOwnerInfo(
                request.OwnerName,
                request.OwnerEmail,
                request.OwnerPhone
            );
        }

        // Update UserId when provided (permite associar ou desassociar a fazenda do usuário)
        farm.SetUserId(request.UserId);

        // Save changes
        await _repository.UpdateAsync(farm, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Map Entity → DTO using AutoMapper
        var farmDto = _mapper.Map<Models.FarmDto>(farm);

        _logger.LogInformation("Updated farm {FarmId}", farm.Id);

        return Result<Models.FarmDto>.Success(farmDto);
    }
}
