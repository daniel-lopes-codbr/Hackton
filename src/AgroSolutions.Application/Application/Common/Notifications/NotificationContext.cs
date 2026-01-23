namespace AgroSolutions.Application.Common.Notifications;

/// <summary>
/// Context for collecting notifications during request processing
/// </summary>
public class NotificationContext
{
    private readonly List<Notification> _notifications = new();
    
    public IReadOnlyCollection<Notification> Notifications => _notifications.AsReadOnly();
    public bool HasNotifications => _notifications.Any();
    
    public void AddNotification(string key, string message)
    {
        _notifications.Add(new Notification(key, message));
    }
    
    public void AddNotification(Notification notification)
    {
        _notifications.Add(notification);
    }
    
    public void Clear()
    {
        _notifications.Clear();
    }
}
