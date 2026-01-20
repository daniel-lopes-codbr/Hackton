namespace AgroSolutions.Domain.Entities;

/// <summary>
/// Represents a user entity with authentication and authorization
/// </summary>
public class User : Entity
{
    public string Name { get; private set; }
    public string Email { get; private set; }
    public string PasswordHash { get; private set; }
    public string Role { get; private set; } // "Admin" or "User"

    private User() { } // For EF Core

    public User(string name, string email, string passwordHash, string role)
        : base()
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("User name cannot be null or empty", nameof(name));

        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("User email cannot be null or empty", nameof(email));

        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new ArgumentException("Password hash cannot be null or empty", nameof(passwordHash));

        if (string.IsNullOrWhiteSpace(role))
            throw new ArgumentException("User role cannot be null or empty", nameof(role));

        if (role != "Admin" && role != "User")
            throw new ArgumentException("User role must be either 'Admin' or 'User'", nameof(role));

        Name = name;
        Email = email;
        PasswordHash = passwordHash;
        Role = role;
    }

    public void UpdateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("User name cannot be null or empty", nameof(name));

        Name = name;
        MarkAsUpdated();
    }

    public void UpdateEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("User email cannot be null or empty", nameof(email));

        Email = email;
        MarkAsUpdated();
    }

    public void UpdatePasswordHash(string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new ArgumentException("Password hash cannot be null or empty", nameof(passwordHash));

        PasswordHash = passwordHash;
        MarkAsUpdated();
    }

    public void UpdateRole(string role)
    {
        if (string.IsNullOrWhiteSpace(role))
            throw new ArgumentException("User role cannot be null or empty", nameof(role));

        if (role != "Admin" && role != "User")
            throw new ArgumentException("User role must be either 'Admin' or 'User'", nameof(role));

        Role = role;
        MarkAsUpdated();
    }
}
