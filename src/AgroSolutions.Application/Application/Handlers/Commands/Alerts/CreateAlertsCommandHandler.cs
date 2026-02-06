using AgroSolutions.Application.Commands.Alerts;
using AgroSolutions.Application.Common.Notifications;
using AgroSolutions.Application.Common.Results;
using AgroSolutions.Domain.Entities;
using AgroSolutions.Domain.Enums;
using AgroSolutions.Domain.Repositories;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AgroSolutions.Application.Handlers.Commands.Alerts;

/// <summary>
/// Handler for CreateAlertsCommand - generates alerts based on sensor readings from last hour
/// </summary>
public class CreateAlertsCommandHandler : IRequestHandler<CreateAlertsCommand, Result<Models.AlertCreationResponseDto>>
{
    private readonly ISensorReadingRepository _sensorReadingRepository;
    private readonly IFieldRepository _fieldRepository;
    private readonly IAlertRepository _alertRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<CreateAlertsCommandHandler> _logger;
    private readonly NotificationContext _notificationContext;

    private const string SoilMoistureSensorType = "SoilMoisture";
    private const decimal DroughtThreshold = 30.0m; // 30%
    private const int DroughtDurationHours = 24;

    public CreateAlertsCommandHandler(
        ISensorReadingRepository sensorReadingRepository,
        IFieldRepository fieldRepository,
        IAlertRepository alertRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<CreateAlertsCommandHandler> logger,
        NotificationContext notificationContext)
    {
        _sensorReadingRepository = sensorReadingRepository;
        _fieldRepository = fieldRepository;
        _alertRepository = alertRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
        _notificationContext = notificationContext;
    }

    public async Task<Result<Models.AlertCreationResponseDto>> Handle(CreateAlertsCommand request, CancellationToken cancellationToken)
    {
        var startTime = DateTime.UtcNow;
        var response = new Models.AlertCreationResponseDto
        {
            Success = true,
            AlertsCreated = 0,
            FieldsProcessed = 0,
            Errors = new List<string>()
        };

        try
        {
            // Query sensor readings from the last hour
            var oneHourAgo = DateTime.UtcNow.AddHours(-1);
            var recentReadings = await GetSensorReadingsFromLastHourAsync(oneHourAgo, cancellationToken);

            if (!recentReadings.Any())
            {
                _logger.LogInformation("No sensor readings found from the last hour");
                response.Success = true;
                response.ProcessingTime = DateTime.UtcNow - startTime;
                return Result<Models.AlertCreationResponseDto>.Success(response);
            }

            // Group readings by FieldId
            var readingsByField = recentReadings.GroupBy(r => r.FieldId);

            var alertsToCreate = new List<Alert>();

            foreach (var fieldGroup in readingsByField)
            {
                var fieldId = fieldGroup.Key;
                response.FieldsProcessed++;

                try
                {
                    // Get field to retrieve FarmId
                    var field = await _fieldRepository.GetByIdAsync(fieldId, cancellationToken);
                    if (field == null)
                    {
                        _logger.LogWarning("Field {FieldId} not found, skipping alert generation", fieldId);
                        response.Errors!.Add($"Field {fieldId} not found");
                        continue;
                    }

                    // Check for Drought Alert (Soil Moisture < 30% in the last 24 hours)
                    var droughtAlert = await CheckDroughtConditionAsync(fieldId, cancellationToken);
                    if (droughtAlert)
                    {
                        alertsToCreate.Add(new Alert(fieldId, AlertStatus.DroughtAlert, true));
                        response.AlertsCreated++;
                    }

                    // Check for Pest Risk (based on air temperature or IsRichInPests flag)
                    var pestRiskAlert = CheckPestRiskCondition(fieldGroup.ToList());
                    if (pestRiskAlert)
                    {
                        alertsToCreate.Add(new Alert(fieldId, AlertStatus.PestRisk, true));
                        response.AlertsCreated++;
                    }

                    // If no alerts, create Normal status alert (optional - based on requirements)
                    // Uncomment if needed:
                    // if (!droughtAlert && !pestRiskAlert)
                    // {
                    //     alertsToCreate.Add(new Alert(
                    //         fieldId,
                    //         field.FarmId,
                    //         AlertStatus.Normal,
                    //         "Field conditions are normal"
                    //     ));
                    // }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing alerts for field {FieldId}", fieldId);
                    response.Errors!.Add($"Error processing field {fieldId}: {ex.Message}");
                }
            }

            // Save all alerts in batch
            if (alertsToCreate.Any())
            {
                await _alertRepository.AddRangeAsync(alertsToCreate, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                _logger.LogInformation("Created {Count} alerts for {FieldsCount} fields", alertsToCreate.Count, response.FieldsProcessed);
            }

            response.ProcessingTime = DateTime.UtcNow - startTime;
            response.Success = true;

            return Result<Models.AlertCreationResponseDto>.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating alerts");
            _notificationContext.AddNotification("Alert", $"Failed to create alerts: {ex.Message}");
            response.Success = false;
            response.Errors!.Add(ex.Message);
            response.ProcessingTime = DateTime.UtcNow - startTime;
            return Result<Models.AlertCreationResponseDto>.Failure(_notificationContext.Notifications);
        }
    }

    /// <summary>
    /// Get sensor readings from the last hour
    /// </summary>
    private async Task<IEnumerable<SensorReading>> GetSensorReadingsFromLastHourAsync(DateTime oneHourAgo, CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;
        return await _sensorReadingRepository.GetByTimestampRangeAsync(oneHourAgo, now, cancellationToken);
    }

    /// <summary>
    /// Check if drought condition exists (soil moisture < 30% in the last 24 hours)
    /// </summary>
    private async Task<bool> CheckDroughtConditionAsync(Guid fieldId, CancellationToken cancellationToken)
    {
        var twentyFourHoursAgo = DateTime.UtcNow.AddHours(-DroughtDurationHours);
        var readings = await _sensorReadingRepository.GetByFieldIdAsync(fieldId, cancellationToken);

        var recentReadings = readings
            .Where(r => r.CreatedAt >= twentyFourHoursAgo && r.SoilMoisture.HasValue)
            .ToList();

        if (!recentReadings.Any())
            return false;

        var allBelowThreshold = recentReadings.All(r => r.SoilMoisture!.Value < DroughtThreshold);
        return allBelowThreshold;
    }

    /// <summary>
    /// Check for pest risk conditions based on air temperature or explicit pest flag
    /// </summary>
    private bool CheckPestRiskCondition(List<SensorReading> readings)
    {
        // If any reading has IsRichInPests true, we consider pest risk
        if (readings.Any(r => r.IsRichInPests == true))
            return true;

        // Check average air temperature over the readings (if available)
        var tempValues = readings.Where(r => r.AirTemperature.HasValue).Select(r => r.AirTemperature!.Value).ToList();
        if (tempValues.Any())
        {
            var avgTemp = tempValues.Average();
            if (avgTemp > 30m) // threshold for pest risk
                return true;
        }

        return false;
    }
}
