namespace AgroSolutions.Application.Common.Notifications;

/// <summary>
/// Represents a notification (error or warning message)
/// </summary>
public class Notification
{
    public string Key { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;

    public Notification(string key, string message)
    {
        Key = key;
        Message = message;
    }
}
