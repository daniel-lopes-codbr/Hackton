using AgroSolutions.Application.Commands.Ingestion;
using Microsoft.Extensions.Logging;
using AgroSolutions.Application.Common.Notifications;
using AgroSolutions.Application.Common.Results;
using AgroSolutions.Domain.Entities;
using AgroSolutions.Domain.Repositories;
using AutoMapper;
using MediatR;

namespace AgroSolutions.Application.Handlers.Commands.Ingestion;

/// <summary>
/// Handler for IngestSingleCommand
/// </summary>
public class IngestSingleCommandHandler : IRequestHandler<IngestSingleCommand, Result<Models.SensorReadingDto>>
{
    private readonly ISensorReadingRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<IngestSingleCommandHandler> _logger;
    private readonly NotificationContext _notificationContext;

    public IngestSingleCommandHandler(
        ISensorReadingRepository repository,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<IngestSingleCommandHandler> logger,
        NotificationContext notificationContext)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
        _notificationContext = notificationContext;
    }

    public async Task<Result<Models.SensorReadingDto>> Handle(IngestSingleCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Create Entity - support both old single-sensor shape and new aggregated telemetry
            SensorReading reading;
            if (!string.IsNullOrWhiteSpace(request.Reading.SensorType))
            {
                // Old-style single sensor reading
                reading = new SensorReading(
                    request.Reading.FieldId,
                    request.Reading.SensorType!,
                    request.Reading.Value ?? 0m,
                    request.Reading.Unit ?? string.Empty,
                    request.Reading.ReadingTimestamp ?? DateTime.UtcNow,
                    request.Reading.Location,
                    request.Reading.Metadata
                );
            }
            else
            {
                // Aggregated telemetry
                reading = new SensorReading(
                    request.Reading.FieldId,
                    request.Reading.SoilMoisture,
                    request.Reading.AirTemperature,
                    request.Reading.Precipitation,
                    request.Reading.IsRichInPests
                );
            }

            // Save
            await _repository.AddAsync(reading, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Map Entity → DTO using AutoMapper
            var readingDto = _mapper.Map<Models.SensorReadingDto>(reading);

            _logger.LogInformation("Ingested single telemetry for Field {FieldId}", reading.FieldId);

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
