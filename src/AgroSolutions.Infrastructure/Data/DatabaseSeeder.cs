using AgroSolutions.Domain.Entities;
using AgroSolutions.Domain.Repositories;
using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
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
            // Ensure database is ready
            if (!await _context.Database.CanConnectAsync())
            {
                _logger.LogWarning("Cannot connect to database. Skipping seed.");
                return;
            }

            // Verify that Users table exists (for SQLite, this ensures table is created)
            try
            {
                // Try to query the table to ensure it exists
                var testQuery = await _context.Users.FirstOrDefaultAsync();
                _logger.LogDebug("Users table is accessible");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Users table may not exist. Error: {ErrorMessage}", ex.Message);
                // If table doesn't exist, try to ensure it's created
                await _context.Database.EnsureCreatedAsync();
            }

            // Check if any users exist (using async method)
            var userCount = await _context.Users.CountAsync();
            _logger.LogInformation("Current user count in database: {UserCount}", userCount);
            
            if (userCount == 0)
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
                var saved = await _context.SaveChangesAsync();

                if (saved > 0)
                {
                    // Verify the user was actually saved
                    var verifyCount = await _context.Users.CountAsync();
                    _logger.LogInformation("Initial admin user created successfully. Total users now: {UserCount}. Email: admin@agrosolutions.com, Password: Admin123!", verifyCount);
                }
                else
                {
                    _logger.LogWarning("Failed to save admin user. No changes were saved. SaveChangesAsync returned {SavedCount}", saved);
                }
            }
            else
            {
                _logger.LogInformation("Users already exist ({UserCount} users). Skipping seed.", userCount);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error seeding database: {ErrorMessage}. StackTrace: {StackTrace}", ex.Message, ex.StackTrace);
            // Don't throw - allow application to continue even if seeding fails
            // This prevents the entire application from failing to start
        }
    }
}
