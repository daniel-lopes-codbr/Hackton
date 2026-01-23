using AgroSolutions.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AgroSolutions.IntegrationTests;

/// <summary>
/// Base class for integration tests with database setup
/// </summary>
public abstract class IntegrationTestBase : IDisposable
{
    protected AgroSolutionsDbContext Context { get; private set; }
    protected IServiceProvider ServiceProvider { get; private set; }
    private readonly DbContextOptions<AgroSolutionsDbContext> _options;
    private bool _disposed = false;

    protected IntegrationTestBase()
    {
        // Create a unique database name for each test to ensure isolation
        var databaseName = $"AgroSolutions_Test_{Guid.NewGuid()}";
        
        _options = new DbContextOptionsBuilder<AgroSolutionsDbContext>()
            .UseInMemoryDatabase(databaseName: databaseName)
            .Options;

        Context = new AgroSolutionsDbContext(_options);
        
        // Create service provider for dependency injection
        var services = new ServiceCollection();
        ConfigureServices(services);
        ServiceProvider = services.BuildServiceProvider();
    }

    /// <summary>
    /// Override this method to configure additional services for tests
    /// </summary>
    protected virtual void ConfigureServices(IServiceCollection services)
    {
        // Base services can be added here if needed
    }

    /// <summary>
    /// Seed test data into the database
    /// </summary>
    protected virtual async Task SeedDatabaseAsync()
    {
        // Override in derived classes to add test data
        await Task.CompletedTask;
    }

    /// <summary>
    /// Clean up the database after each test
    /// </summary>
    protected virtual async Task CleanupDatabaseAsync()
    {
        Context.Database.EnsureDeleted();
        await Context.Database.EnsureCreatedAsync();
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            Context?.Dispose();
            if (ServiceProvider is IDisposable disposable)
            {
                disposable.Dispose();
            }
            _disposed = true;
        }
    }
}
