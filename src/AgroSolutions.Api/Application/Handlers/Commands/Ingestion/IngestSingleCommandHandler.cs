using AgroSolutions.Api.Application.Commands.Ingestion;
using AgroSolutions.Api.Application.Common.Notifications;
using AgroSolutions.Api.Application.Common.Results;
using AgroSolutions.Domain.Entities;
using AgroSolutions.Domain.Repositories;
using AutoMapper;
using MediatR;

namespace AgroSolutions.Api.Application.Handlers.Commands.Ingestion;

/// <summary>
/// Handler for IngestSingleCommand
/// </summary>
public class IngestSingleCommandHandler : IRequestHandler<IngestSingleCommand, Result<Models.SensorReadingDto>>
{
    private readonly ISensorReadingRepository _repository;
    private readonly IMapper _mapper;
    private readonly ILogger<IngestSingleCommandHandler> _logger;
    private readonly NotificationContext _notificationContext;

    public IngestSingleCommandHandler(
        ISensorReadingRepository repository,
        IMapper mapper,
        ILogger<IngestSingleCommandHandler> logger,
        NotificationContext notificationContext)
    {
        _repository = repository;
        _mapper = mapper;
        _logger = logger;
        _notificationContext = notificationContext;
    }

    public async Task<Result<Models.SensorReadingDto>> Handle(IngestSingleCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Create Entity
            var reading = new SensorReading(
                request.Reading.FieldId,
                request.Reading.SensorType,
                request.Reading.Value,
                request.Reading.Unit,
                request.Reading.ReadingTimestamp,
                request.Reading.Location,
                request.Reading.Metadata
            );

            // Save
            await _repository.AddAsync(reading, cancellationToken);
            await _repository.SaveChangesAsync(cancellationToken);

            // Map Entity → DTO using AutoMapper
            var readingDto = _mapper.Map<Models.SensorReadingDto>(reading);

            _logger.LogInformation("Ingested single reading: {SensorType} for Field {FieldId}", reading.SensorType, reading.FieldId);

            return Result<Models.SensorReadingDto>.Success(readingDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error ingesting single reading for Field {FieldId}", request.Reading.FieldId);
            _notificationContext.AddNotification("Ingestion", $"Failed to ingest sensor reading: {ex.Message}");
            return Result<Models.SensorReadingDto>.Failure(_notificationContext.Notifications);
        }
    }
}
