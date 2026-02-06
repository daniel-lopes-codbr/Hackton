using AgroSolutions.Application.Commands.Farms;
using Microsoft.Extensions.Logging;
using AgroSolutions.Application.Common.Notifications;
using AgroSolutions.Application.Common.Results;
using AgroSolutions.Domain.Entities;
using AgroSolutions.Domain.Repositories;
using AgroSolutions.Domain.ValueObjects;
using AutoMapper;
using MediatR;

namespace AgroSolutions.Application.Handlers.Commands.Farms;

/// <summary>
/// Handler for CreateFarmCommand
/// </summary>
public class CreateFarmCommandHandler : IRequestHandler<CreateFarmCommand, Result<Models.FarmDto>>
{
    private readonly IFarmRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<CreateFarmCommandHandler> _logger;
    private readonly NotificationContext _notificationContext;

    public CreateFarmCommandHandler(
        IFarmRepository repository,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<CreateFarmCommandHandler> logger,
        NotificationContext notificationContext)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
        _notificationContext = notificationContext;
    }

    public async Task<Result<Models.FarmDto>> Handle(CreateFarmCommand request, CancellationToken cancellationToken)
    {
        // Create Entity using new Farm shape
        var farm = new Farm(
            request.Name,
            request.WidthMeters,
            request.LengthMeters,
            request.Precipitation,
            request.UserId
        );

        // Save
        await _repository.AddAsync(farm, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Map Entity → DTO using AutoMapper
        var farmDto = _mapper.Map<Models.FarmDto>(farm);

        _logger.LogInformation("Created farm {FarmId} with name {Name}", farm.Id, farm.Name);

        return Result<Models.FarmDto>.Success(farmDto);
    }
}
