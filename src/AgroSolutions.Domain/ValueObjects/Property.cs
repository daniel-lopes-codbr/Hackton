namespace AgroSolutions.Domain.ValueObjects;

/// <summary>
/// Value object representing a property (farm/field) in the agricultural domain
/// </summary>
public class Property : IEquatable<Property>
{
    public string Name { get; private set; }
    public string Location { get; private set; }
    public decimal Area { get; private set; } // in hectares
    public string? Description { get; private set; }

    private Property() { } // For EF Core

    public Property(string name, string location, decimal area, string? description = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Property name cannot be null or empty", nameof(name));

        if (string.IsNullOrWhiteSpace(location))
            throw new ArgumentException("Property location cannot be null or empty", nameof(location));

        if (area <= 0)
            throw new ArgumentException("Property area must be greater than zero", nameof(area));

        Name = name;
        Location = location;
        Area = area;
        Description = description;
    }

    public bool Equals(Property? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        
        return Name == other.Name && 
               Location == other.Location && 
               Area == other.Area;
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as Property);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Name, Location, Area);
    }

    public override string ToString()
    {
        return $"{Name} - {Location} ({Area} ha)";
    }
}
