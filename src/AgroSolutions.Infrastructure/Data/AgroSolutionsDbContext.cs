using AgroSolutions.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AgroSolutions.Infrastructure.Data;

/// <summary>
/// DbContext for AgroSolutions application
/// </summary>
public class AgroSolutionsDbContext : DbContext
{
    public AgroSolutionsDbContext(DbContextOptions<AgroSolutionsDbContext> options)
        : base(options)
    {
    }

    public DbSet<Farm> Farms { get; set; }
    public DbSet<Field> Fields { get; set; }
    public DbSet<SensorReading> SensorReadings { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<UserAuthorization> UserAuthorizations { get; set; }
    public DbSet<Alert> Alerts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Detect if using Postgres provider to set appropriate column types
        var isPostgres = Database.ProviderName != null && Database.ProviderName.Contains("Npgsql", System.StringComparison.OrdinalIgnoreCase);

        // Configure Farm
        modelBuilder.Entity<Farm>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedNever(); // We generate GUIDs ourselves
            if (isPostgres) entity.Property(e => e.Id).HasColumnType("uuid");
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.UserId);
            if (isPostgres) entity.Property(e => e.UserId).HasColumnType("uuid");
            entity.Property(e => e.WidthMeters).IsRequired().HasPrecision(18, 2);
            entity.Property(e => e.LengthMeters).IsRequired().HasPrecision(18, 2);
            entity.Property(e => e.TotalAreaSquareMeters).IsRequired().HasPrecision(18, 2);
            entity.Property(e => e.Precipitation).HasPrecision(18, 2);
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt);
            entity.HasIndex(e => e.UserId);
            entity.HasOne<User>()
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);
            // Owned legacy Property value object (kept for backward compatibility)
            entity.OwnsOne(e => e.Property, property =>
            {
                property.Property(p => p.Name).HasMaxLength(200);
                property.Property(p => p.Location).HasMaxLength(500);
                property.Property(p => p.Area).HasPrecision(18, 2);
                property.Property(p => p.Description).HasMaxLength(1000);
            });
        });

        // Configure Field
        modelBuilder.Entity<Field>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedNever();
            if (isPostgres) entity.Property(e => e.Id).HasColumnType("uuid");
            entity.Property(e => e.FarmId).IsRequired();
            if (isPostgres) entity.Property(e => e.FarmId).HasColumnType("uuid");
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.AreaSquareMeters).IsRequired().HasPrecision(18, 2);
            entity.Property(e => e.CropType).IsRequired().HasMaxLength(100);
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt);
            
            entity.HasIndex(e => e.FarmId);
            entity.HasOne<Farm>()
                .WithMany()
                .HasForeignKey(e => e.FarmId)
                .OnDelete(DeleteBehavior.Restrict);
            // Owned legacy Property value object (kept for backward compatibility)
            entity.OwnsOne(e => e.Property, property =>
            {
                property.Property(p => p.Name).HasMaxLength(200);
                property.Property(p => p.Location).HasMaxLength(500);
                property.Property(p => p.Area).HasPrecision(18, 2);
                property.Property(p => p.Description).HasMaxLength(1000);
            });
        });

        // Configure SensorReading (supports both single-sensor and aggregated telemetry)
        modelBuilder.Entity<SensorReading>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedNever();
            if (isPostgres) entity.Property(e => e.Id).HasColumnType("uuid");
            entity.Property(e => e.FieldId).IsRequired();
            if (isPostgres) entity.Property(e => e.FieldId).HasColumnType("uuid");

            // Old fields (single sensor readings)
            entity.Property(e => e.SensorType).HasMaxLength(50);
            entity.Property(e => e.Value).HasPrecision(18, 4);
            entity.Property(e => e.Unit).HasMaxLength(20);
            entity.Property(e => e.ReadingTimestamp);
            entity.Property(e => e.Location).HasMaxLength(200);
            var metadataConversion = entity.Property(e => e.Metadata)
                .HasConversion(
                    v => v == null ? (string?)null : ConvertDictionaryToJson(v),
                    v => string.IsNullOrEmpty(v) ? null : ConvertJsonToDictionary(v));
            if (isPostgres)
            {
                metadataConversion.HasColumnType("jsonb");
            }
            else
            {
                metadataConversion.HasColumnType("TEXT");
            }

            // New aggregated telemetry fields
            entity.Property(e => e.SoilMoisture).HasPrecision(18, 4);
            entity.Property(e => e.AirTemperature).HasPrecision(18, 4);
            entity.Property(e => e.Precipitation).HasPrecision(18, 4);
            entity.Property(e => e.IsRichInPests);

            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt);
            
            entity.HasIndex(e => e.FieldId);
            entity.HasIndex(e => e.ReadingTimestamp);
            entity.HasIndex(e => e.CreatedAt);
        });

        // Configure User
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedNever();
            if (isPostgres) entity.Property(e => e.Id).HasColumnType("uuid");
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(200);
            entity.Property(e => e.PasswordHash).IsRequired().HasMaxLength(500);
            entity.Property(e => e.Role).IsRequired().HasMaxLength(20);
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt);
            
            entity.HasIndex(e => e.Email).IsUnique();
            entity.HasIndex(e => e.Role);
        });

        // Configure Alert
        modelBuilder.Entity<Alert>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedNever();
            if (isPostgres) entity.Property(e => e.Id).HasColumnType("uuid");
            entity.Property(e => e.FieldId).IsRequired();
            if (isPostgres) entity.Property(e => e.FieldId).HasColumnType("uuid");
            entity.Property(e => e.Status).IsRequired().HasConversion<int>();
            entity.Property(e => e.IsEnable).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt);
            
            entity.HasIndex(e => e.FieldId);
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.IsEnable);
            entity.HasIndex(e => e.CreatedAt);
        });
    }

    private static string? ConvertDictionaryToJson(Dictionary<string, string>? dictionary)
    {
        if (dictionary == null)
            return null;
        
        return System.Text.Json.JsonSerializer.Serialize(dictionary);
    }

    private static Dictionary<string, string>? ConvertJsonToDictionary(string? json)
    {
        if (string.IsNullOrEmpty(json))
            return null;
        
        return System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(json);
    }
}
