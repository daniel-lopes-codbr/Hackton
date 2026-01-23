using AgroSolutions.Api;
using AgroSolutions.Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AgroSolutions.IntegrationTests;

/// <summary>
/// Custom WebApplicationFactory for integration tests with E2E scenarios
/// </summary>
public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove the real database context
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<AgroSolutionsDbContext>));

            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            // Add in-memory database for testing
            services.AddDbContext<AgroSolutionsDbContext>(options =>
            {
                options.UseInMemoryDatabase($"AgroSolutions_Test_{Guid.NewGuid()}");
            });

            // Ensure database is created
            var sp = services.BuildServiceProvider();
            using (var scope = sp.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var db = scopedServices.GetRequiredService<AgroSolutionsDbContext>();
                db.Database.EnsureCreated();
            }
        });

        builder.UseEnvironment("Testing");
    }
}
