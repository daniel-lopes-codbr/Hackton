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
/// Handler for IngestBatchParallelCommand (parallel processing)
/// </summary>
public class IngestBatchParallelCommandHandler : IRequestHandler<IngestBatchParallelCommand, Result<Models.IngestionResponseDto>>
{
    private readonly ISensorReadingRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<IngestBatchParallelCommandHandler> _logger;
    private readonly NotificationContext _notificationContext;

    public IngestBatchParallelCommandHandler(
        ISensorReadingRepository repository,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<IngestBatchParallelCommandHandler> logger,
        NotificationContext notificationContext)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
        _notificationContext = notificationContext;
    }

    public async Task<Result<Models.IngestionResponseDto>> Handle(IngestBatchParallelCommand request, CancellationToken cancellationToken)
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

        // Process readings in parallel
        var readingsToAdd = new System.Collections.Concurrent.ConcurrentBag<SensorReading>();
        var errors = new System.Collections.Concurrent.ConcurrentBag<string>();
        int processedCount = 0;
        int failedCount = 0;

        await Parallel.ForEachAsync(
            request.Batch.Readings,
            new ParallelOptions
            {
                MaxDegreeOfParallelism = Environment.ProcessorCount,
                CancellationToken = cancellationToken
            },
            (dto, ct) =>
            {
                try
                {
                    SensorReading reading;
                    if (!string.IsNullOrWhiteSpace(dto.SensorType))
                    {
                        reading = new SensorReading(
                            dto.FieldId,
                            dto.SensorType!,
                            dto.Value ?? 0m,
                            dto.Unit ?? string.Empty,
                            dto.ReadingTimestamp ?? DateTime.UtcNow,
                            dto.Location,
                            dto.Metadata
                        );
                    }
                    else
                    {
                        reading = new SensorReading(
                            dto.FieldId,
                            dto.SoilMoisture,
                            dto.AirTemperature,
                            dto.Precipitation,
                            dto.IsRichInPests
                        );
                    }
                    readingsToAdd.Add(reading);
                    Interlocked.Increment(ref processedCount);
                }
                catch (Exception ex)
                {
                    Interlocked.Increment(ref failedCount);
                    errors.Add($"Field {dto.FieldId}: {ex.Message}");
                    _logger.LogWarning(ex, "Failed to create reading for Field {FieldId}", dto.FieldId);
                }
                return ValueTask.CompletedTask;
            });

        response.ProcessedCount = processedCount;
        response.FailedCount = failedCount;
        response.Errors = errors.ToList();

        // Add all readings in batch
        if (readingsToAdd.Any())
        {
            try
            {
                await _repository.AddRangeAsync(readingsToAdd.ToList(), cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                response.Success = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving parallel batch of readings");
                _notificationContext.AddNotification("Batch", $"Failed to save batch: {ex.Message}");
                response.Success = false;
                response.Errors!.Add($"Batch save failed: {ex.Message}");
            }
        }

        response.ProcessingTime = DateTime.UtcNow - startTime;
        _logger.LogInformation("Ingested parallel batch: {ProcessedCount} processed, {FailedCount} failed in {ProcessingTime}ms",
            response.ProcessedCount, response.FailedCount, response.ProcessingTime.TotalMilliseconds);

        if (!response.Success)
            return Result<Models.IngestionResponseDto>.Failure(_notificationContext.Notifications);

        return Result<Models.IngestionResponseDto>.Success(response);
    }
}
