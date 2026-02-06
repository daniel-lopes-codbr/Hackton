namespace AgroSolutions.Domain.Entities;

/// <summary>
/// Represents a user authorization/permission
/// </summary>
public class UserAuthorization : Entity
{
    public Guid UserId { get; private set; }
    public string PermissionType { get; private set; } = string.Empty;

    private UserAuthorization() { } // For EF Core

    public UserAuthorization(Guid userId, string permissionType)
        : base()
    {
        if (userId == Guid.Empty) throw new ArgumentException("User ID cannot be empty", nameof(userId));
        if (string.IsNullOrWhiteSpace(permissionType)) throw new ArgumentException("Permission type cannot be null or empty", nameof(permissionType));

        UserId = userId;
        PermissionType = permissionType;
    }

    public void UpdatePermission(string permissionType)
    {
        if (string.IsNullOrWhiteSpace(permissionType)) throw new ArgumentException("Permission type cannot be null or empty", nameof(permissionType));
        PermissionType = permissionType;
        MarkAsUpdated();
    }
}

