using AgroSolutions.Domain.ValueObjects;

namespace AgroSolutions.Domain.Entities;

/// <summary>
/// Represents a farm entity with identity and properties
/// </summary>
public class Farm : Entity
{
    public Property Property { get; private set; }
    public Guid? UserId { get; private set; }
    public string OwnerName { get; private set; }
    public string? OwnerEmail { get; private set; }
    public string? OwnerPhone { get; private set; }

    private Farm() { } // For EF Core

    public Farm(Property property, string ownerName, string? ownerEmail = null, string? ownerPhone = null, Guid? userId = null)
        : base()
    {
        if (string.IsNullOrWhiteSpace(ownerName))
            throw new ArgumentException("Owner name cannot be null or empty", nameof(ownerName));

        Property = property ?? throw new ArgumentNullException(nameof(property));
        UserId = userId;
        OwnerName = ownerName;
        OwnerEmail = ownerEmail;
        OwnerPhone = ownerPhone;
    }

    public void UpdateProperty(Property newProperty)
    {
        Property = newProperty ?? throw new ArgumentNullException(nameof(newProperty));
        MarkAsUpdated();
    }

    public void UpdateOwnerInfo(string ownerName, string? ownerEmail = null, string? ownerPhone = null)
    {
        if (string.IsNullOrWhiteSpace(ownerName))
            throw new ArgumentException("Owner name cannot be null or empty", nameof(ownerName));

        OwnerName = ownerName;
        OwnerEmail = ownerEmail;
        OwnerPhone = ownerPhone;
        MarkAsUpdated();
    }

    public void SetUserId(Guid? userId)
    {
        UserId = userId;
        MarkAsUpdated();
    }
}
