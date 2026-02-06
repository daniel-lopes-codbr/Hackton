namespace AgroSolutions.Application.Models;

/// <summary>
/// DTO for UserAuthorization
/// </summary>
public class UserAuthorizationDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string PermissionType { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? LastUpdatedAt { get; set; }
}

public class CreateUserAuthorizationDto
{
    public Guid UserId { get; set; }
    public string PermissionType { get; set; } = string.Empty;
}

public class UpdateUserAuthorizationDto
{
    public string? PermissionType { get; set; }
}

