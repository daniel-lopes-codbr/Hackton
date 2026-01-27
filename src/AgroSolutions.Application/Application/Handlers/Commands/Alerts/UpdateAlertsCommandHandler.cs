using AgroSolutions.Application.Commands.Alerts;
using AgroSolutions.Application.Common.Notifications;
using AgroSolutions.Application.Common.Results;
using AgroSolutions.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AgroSolutions.Application.Handlers.Commands.Alerts;

/// <summary>
/// Handler for UpdateAlertsCommand - deactivates alerts created the previous day
/// </summary>
public class UpdateAlertsCommandHandler : IRequestHandler<UpdateAlertsCommand, Result<int>>
{
    private readonly IAlertRepository _alertRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateAlertsCommandHandler> _logger;
    private readonly NotificationContext _notificationContext;

    public UpdateAlertsCommandHandler(
        IAlertRepository alertRepository,
        IUnitOfWork unitOfWork,
        ILogger<UpdateAlertsCommandHandler> logger,
        NotificationContext notificationContext)
    {
        _alertRepository = alertRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
        _notificationContext = notificationContext;
    }

    public async Task<Result<int>> Handle(UpdateAlertsCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Get the start of yesterday (00:00:00)
            var yesterdayStart = DateTime.UtcNow.Date.AddDays(-1);
            // Get the end of yesterday (23:59:59)
            var yesterdayEnd = yesterdayStart.AddDays(1).AddTicks(-1);

            // Get all active alerts created yesterday
            var alertsToDeactivate = await _alertRepository.GetAlertsCreatedBeforeAsync(yesterdayEnd, cancellationToken);
            var alertsList = alertsToDeactivate.Where(a => a.CreatedAt >= yesterdayStart && a.CreatedAt <= yesterdayEnd).ToList();

            if (!alertsList.Any())
            {
                _logger.LogInformation("No alerts found from previous day to deactivate");
                return Result<int>.Success(0);
            }

            // Deactivate all alerts
            foreach (var alert in alertsList)
            {
                alert.Deactivate();
            }

            // Update in batch
            await _alertRepository.UpdateRangeAsync(alertsList, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Deactivated {Count} alerts from previous day", alertsList.Count);

            return Result<int>.Success(alertsList.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating alerts");
            _notificationContext.AddNotification("Alert", $"Failed to update alerts: {ex.Message}");
            return Result<int>.Failure(_notificationContext.Notifications);
        }
    }
}
