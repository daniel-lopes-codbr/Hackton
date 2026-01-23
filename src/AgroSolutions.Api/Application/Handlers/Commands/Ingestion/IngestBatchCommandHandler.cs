using AgroSolutions.Api.Application.Commands.Ingestion;
using AgroSolutions.Api.Application.Common.Notifications;
using AgroSolutions.Api.Application.Common.Results;
using AgroSolutions.Domain.Entities;
using AgroSolutions.Domain.Repositories;
using AutoMapper;
using MediatR;

namespace AgroSolutions.Api.Application.Handlers.Commands.Ingestion;

/// <summary>
/// Handler for IngestBatchCommand (sequential processing)
/// </summary>
public class IngestBatchCommandHandler : IRequestHandler<IngestBatchCommand, Result<Models.IngestionResponseDto>>
{
    private readonly ISensorReadingRepository _repository;
    private readonly IMapper _mapper;
    private readonly ILogger<IngestBatchCommandHandler> _logger;
    private readonly NotificationContext _notificationContext;

    public IngestBatchCommandHandler(
        ISensorReadingRepository repository,
        IMapper mapper,
        ILogger<IngestBatchCommandHandler> logger,
        NotificationContext notificationContext)
    {
        _repository = repository;
        _mapper = mapper;
        _logger = logger;
        _notificationContext = notificationContext;
    }

    public async Task<Result<Models.IngestionResponseDto>> Handle(IngestBatchCommand request, CancellationToken cancellationToken)
    {
        var startTime = DateTime.UtcNow;
        var response = new Models.IngestionResponseDto
        {
            Success = true,
            Errors = new List<string>()
        };

        if (request.Batch.Readings == null || request.Batch.Readings.Count == 0)
        {
            _notificationContext.AddNotification("Batch", "No readings provided in batch");
            response.Success = false;
            response.Errors!.Add("No readings provided in batch");
            return Result<Models.IngestionResponseDto>.Failure(_notificationContext.Notifications);
        }

        var readingsToAdd = new List<SensorReading>();

        foreach (var dto in request.Batch.Readings)
        {
            if (cancellationToken.IsCancellationRequested)
                break;

            try
            {
                var reading = new SensorReading(
                    dto.FieldId,
                    dto.SensorType,
                    dto.Value,
                    dto.Unit,
                    dto.ReadingTimestamp,
                    dto.Location,
                    dto.Metadata
                );
                readingsToAdd.Add(reading);
                response.ProcessedCount++;
            }
            catch (Exception ex)
            {
                response.FailedCount++;
                response.Errors!.Add($"Field {dto.FieldId}: {ex.Message}");
                _logger.LogWarning(ex, "Failed to create reading for Field {FieldId}", dto.FieldId);
            }
        }

        // Add all readings in batch
        if (readingsToAdd.Any())
        {
            try
            {
                await _repository.AddRangeAsync(readingsToAdd, cancellationToken);
                await _repository.SaveChangesAsync(cancellationToken);
                response.Success = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving batch of readings");
                _notificationContext.AddNotification("Batch", $"Failed to save batch: {ex.Message}");
                response.Success = false;
                response.Errors!.Add($"Batch save failed: {ex.Message}");
            }
        }

        response.ProcessingTime = DateTime.UtcNow - startTime;
        _logger.LogInformation("Ingested batch: {ProcessedCount} processed, {FailedCount} failed in {ProcessingTime}ms",
            response.ProcessedCount, response.FailedCount, response.ProcessingTime.TotalMilliseconds);

        if (!response.Success)
            return Result<Models.IngestionResponseDto>.Failure(_notificationContext.Notifications);

        return Result<Models.IngestionResponseDto>.Success(response);
    }
}
