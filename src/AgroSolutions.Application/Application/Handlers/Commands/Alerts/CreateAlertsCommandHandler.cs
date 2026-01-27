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

                    // Check for Drought Alert (Soil Moisture < 30% for more than 24 hours)
                    var droughtAlert = await CheckDroughtConditionAsync(fieldId, cancellationToken);
                    if (droughtAlert != null)
                    {
                        alertsToCreate.Add(new Alert(
                            fieldId,
                            field.FarmId,
                            AlertStatus.DroughtAlert,
                            droughtAlert
                        ));
                        response.AlertsCreated++;
                    }

                    // Check for Pest Risk (based on air humidity or temperature)
                    var pestRiskAlert = CheckPestRiskCondition(fieldGroup.ToList());
                    if (pestRiskAlert != null)
                    {
                        alertsToCreate.Add(new Alert(
                            fieldId,
                            field.FarmId,
                            AlertStatus.PestRisk,
                            pestRiskAlert
                        ));
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
    /// Check if drought condition exists (soil moisture < 30% for more than 24 hours)
    /// </summary>
    private async Task<string?> CheckDroughtConditionAsync(Guid fieldId, CancellationToken cancellationToken)
    {
        // Get all soil moisture readings for this field
        var soilMoistureReadings = await _sensorReadingRepository.GetByFieldIdAndSensorTypeAsync(
            fieldId, 
            SoilMoistureSensorType, 
            cancellationToken);

        if (!soilMoistureReadings.Any())
            return null;

        // Order by timestamp descending (most recent first)
        var orderedReadings = soilMoistureReadings
            .OrderByDescending(r => r.ReadingTimestamp)
            .ToList();

        // Check if we have readings spanning more than 24 hours
        var oldestReading = orderedReadings.Last();
        var newestReading = orderedReadings.First();
        var timeSpan = newestReading.ReadingTimestamp - oldestReading.ReadingTimestamp;

        if (timeSpan.TotalHours < DroughtDurationHours)
            return null; // Not enough data for 24-hour analysis

        // Check if all readings in the last 24 hours are below threshold
        var twentyFourHoursAgo = DateTime.UtcNow.AddHours(-DroughtDurationHours);
        var recentReadings = orderedReadings
            .Where(r => r.ReadingTimestamp >= twentyFourHoursAgo)
            .ToList();

        if (!recentReadings.Any())
            return null;

        // Check if all readings are below threshold
        var allBelowThreshold = recentReadings.All(r => r.Value < DroughtThreshold);

        if (allBelowThreshold)
        {
            var lowestValue = recentReadings.Min(r => r.Value);
            return $"Soil moisture below {DroughtThreshold}% for more than {DroughtDurationHours} hours. Current value: {lowestValue:F2}%";
        }

        return null;
    }

    /// <summary>
    /// Check for pest risk conditions based on air humidity or temperature
    /// </summary>
    private string? CheckPestRiskCondition(List<SensorReading> readings)
    {
        // Check for high humidity (favorable for pests)
        var humidityReadings = readings
            .Where(r => r.SensorType.Equals("Humidity", StringComparison.OrdinalIgnoreCase) ||
                       r.SensorType.Equals("AirHumidity", StringComparison.OrdinalIgnoreCase))
            .ToList();

        if (humidityReadings.Any())
        {
            var avgHumidity = humidityReadings.Average(r => r.Value);
            // High humidity (> 80%) can indicate pest risk
            if (avgHumidity > 80m)
            {
                return $"High air humidity detected ({avgHumidity:F2}%). Conditions favorable for pest development.";
            }
        }

        // Check metadata for temperature and humidity indicators
        foreach (var reading in readings)
        {
            if (reading.Metadata != null)
            {
                // Check for temperature in metadata
                if (reading.Metadata.TryGetValue("Temperature", out var tempStr) ||
                    reading.Metadata.TryGetValue("temperature", out tempStr))
                {
                    if (decimal.TryParse(tempStr, out var temperature))
                    {
                        // High temperature (> 30°C) combined with humidity can indicate pest risk
                        if (temperature > 30m && humidityReadings.Any(h => h.Value > 70m))
                        {
                            return $"High temperature ({temperature:F2}°C) and humidity detected. Pest risk conditions present.";
                        }
                    }
                }

                // Check for humidity in metadata
                if (reading.Metadata.TryGetValue("Humidity", out var humStr) ||
                    reading.Metadata.TryGetValue("humidity", out humStr))
                {
                    if (decimal.TryParse(humStr, out var humidity) && humidity > 80m)
                    {
                        return $"High humidity detected in metadata ({humidity:F2}%). Pest risk conditions present.";
                    }
                }
            }
        }

        return null;
    }
}
