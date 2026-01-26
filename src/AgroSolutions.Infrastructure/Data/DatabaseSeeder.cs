using AgroSolutions.Domain.Entities;
using AgroSolutions.Domain.Repositories;
using BCrypt.Net;
using Microsoft.Extensions.Logging;

namespace AgroSolutions.Infrastructure.Data;

/// <summary>
/// Seeds initial data into the database
/// </summary>
public class DatabaseSeeder
{
    private readonly AgroSolutionsDbContext _context;
    private readonly ILogger<DatabaseSeeder> _logger;

    public DatabaseSeeder(AgroSolutionsDbContext context, ILogger<DatabaseSeeder> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Seeds the database with initial admin user if no users exist
    /// </summary>
    public async Task SeedAsync()
    {
        try
        {
            // Check if any users exist
            if (!_context.Users.Any())
            {
                _logger.LogInformation("No users found. Seeding initial admin user...");

                // Create default admin user
                var adminPasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!");
                var adminUser = new User(
                    name: "Admin User",
                    email: "admin@agrosolutions.com",
                    passwordHash: adminPasswordHash,
                    role: "Admin"
                );

                _context.Users.Add(adminUser);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Initial admin user created successfully. Email: admin@agrosolutions.com, Password: Admin123!");
            }
            else
            {
                _logger.LogInformation("Users already exist. Skipping seed.");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error seeding database");
            throw;
        }
    }
}
