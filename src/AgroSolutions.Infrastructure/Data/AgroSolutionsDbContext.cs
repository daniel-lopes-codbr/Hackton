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
    public DbSet<Alert> Alerts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Farm
        modelBuilder.Entity<Farm>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedNever(); // We generate GUIDs ourselves
            entity.OwnsOne(e => e.Property, property =>
            {
                property.Property(p => p.Name).IsRequired().HasMaxLength(200);
                property.Property(p => p.Location).IsRequired().HasMaxLength(500);
                property.Property(p => p.Area).IsRequired().HasPrecision(18, 2);
                property.Property(p => p.Description).HasMaxLength(1000);
            });
            entity.Property(e => e.UserId);
            entity.Property(e => e.OwnerName).IsRequired().HasMaxLength(200);
            entity.Property(e => e.OwnerEmail).HasMaxLength(200);
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt);
            entity.HasIndex(e => e.UserId);
            entity.HasOne<User>()
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Configure Field
        modelBuilder.Entity<Field>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.OwnsOne(e => e.Property, property =>
            {
                property.Property(p => p.Name).IsRequired().HasMaxLength(200);
                property.Property(p => p.Location).IsRequired().HasMaxLength(500);
                property.Property(p => p.Area).IsRequired().HasPrecision(18, 2);
                property.Property(p => p.Description).HasMaxLength(1000);
            });
            entity.Property(e => e.FarmId).IsRequired();
            entity.Property(e => e.CropType).IsRequired().HasMaxLength(100);
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt);
            
            entity.HasIndex(e => e.FarmId);
            entity.HasOne<Farm>()
                .WithMany()
                .HasForeignKey(e => e.FarmId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Configure SensorReading
        modelBuilder.Entity<SensorReading>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.FieldId).IsRequired();
            entity.Property(e => e.SensorType).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Value).IsRequired().HasPrecision(18, 4);
            entity.Property(e => e.Unit).IsRequired().HasMaxLength(20);
            entity.Property(e => e.ReadingTimestamp).IsRequired();
            entity.Property(e => e.Location).HasMaxLength(200);
            entity.Property(e => e.Metadata)
                .HasConversion(
                    v => v == null ? (string?)null : ConvertDictionaryToJson(v),
                    v => string.IsNullOrEmpty(v) ? null : ConvertJsonToDictionary(v))
                .HasColumnType("TEXT"); // Use TEXT for SQLite compatibility (works with SQL Server too)
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt);
            
            entity.HasIndex(e => e.FieldId);
            entity.HasIndex(e => e.SensorType);
            entity.HasIndex(e => e.ReadingTimestamp);
            entity.HasIndex(e => new { e.FieldId, e.SensorType, e.ReadingTimestamp });
        });

        // Configure User
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedNever();
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
            entity.Property(e => e.FieldId).IsRequired();
            entity.Property(e => e.FarmId).IsRequired();
            entity.Property(e => e.Status).IsRequired().HasConversion<int>();
            entity.Property(e => e.Message).IsRequired().HasMaxLength(500);
            entity.Property(e => e.IsActive).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt);
            
            entity.HasIndex(e => e.FieldId);
            entity.HasIndex(e => e.FarmId);
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.IsActive);
            entity.HasIndex(e => e.CreatedAt);
            entity.HasIndex(e => new { e.FieldId, e.IsActive });
            entity.HasIndex(e => new { e.FarmId, e.IsActive });
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
