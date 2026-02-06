using AgroSolutions.Domain.ValueObjects;

namespace AgroSolutions.Domain.Entities;

/// <summary>
/// Represents a farm entity aligned with diagram:
/// Id, Name, UserId, WidthMeters, LengthMeters, TotalAreaSquareMeters, Precipitation
/// </summary>
public class Farm : Entity
{
    public string Name { get; private set; }
    public Guid? UserId { get; private set; }
    public decimal WidthMeters { get; private set; }
    public decimal LengthMeters { get; private set; }
    public decimal TotalAreaSquareMeters { get; private set; }
    public decimal? Precipitation { get; private set; }
    // Backwards compatibility
    public Property? Property { get; private set; }
    public string? OwnerName { get; private set; }
    public string? OwnerEmail { get; private set; }
    public string? OwnerPhone { get; private set; }

    private Farm() { } // For EF Core

    public Farm(string name, decimal widthMeters, decimal lengthMeters, decimal? precipitation = null, Guid? userId = null)
        : base()
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Farm name cannot be null or empty", nameof(name));

        if (widthMeters <= 0) throw new ArgumentException("Width must be positive", nameof(widthMeters));
        if (lengthMeters <= 0) throw new ArgumentException("Length must be positive", nameof(lengthMeters));

        Name = name;
        WidthMeters = widthMeters;
        LengthMeters = lengthMeters;
        TotalAreaSquareMeters = widthMeters * lengthMeters;
        Precipitation = precipitation;
        UserId = userId;
    }

    // Legacy constructor for tests/compatibility
    public Farm(Property property, string ownerName, string? ownerEmail = null, string? ownerPhone = null, Guid? userId = null)
        : base()
    {
        if (property == null) throw new ArgumentNullException(nameof(property));
        if (string.IsNullOrWhiteSpace(ownerName)) throw new ArgumentException("Owner name cannot be null or empty", nameof(ownerName));

        Property = property;
        Name = property.Name;
        WidthMeters = 0m;
        LengthMeters = 0m;
        TotalAreaSquareMeters = property.Area;
        OwnerName = ownerName;
        OwnerEmail = ownerEmail;
        OwnerPhone = ownerPhone;
        UserId = userId;
    }

    public void UpdateDimensions(decimal widthMeters, decimal lengthMeters)
    {
        if (widthMeters <= 0) throw new ArgumentException("Width must be positive", nameof(widthMeters));
        if (lengthMeters <= 0) throw new ArgumentException("Length must be positive", nameof(lengthMeters));

        WidthMeters = widthMeters;
        LengthMeters = lengthMeters;
        TotalAreaSquareMeters = widthMeters * lengthMeters;
        MarkAsUpdated();
    }

    public void UpdateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Farm name cannot be null or empty", nameof(name));
        Name = name;
        MarkAsUpdated();
    }

    public void SetPrecipitation(decimal? precipitation)
    {
        Precipitation = precipitation;
        MarkAsUpdated();
    }

    public void SetUserId(Guid? userId)
    {
        UserId = userId;
        MarkAsUpdated();
    }
    // Legacy methods
    public void UpdateProperty(Property newProperty)
    {
        Property = newProperty ?? throw new ArgumentNullException(nameof(newProperty));
        Name = newProperty.Name;
        TotalAreaSquareMeters = newProperty.Area;
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
}
