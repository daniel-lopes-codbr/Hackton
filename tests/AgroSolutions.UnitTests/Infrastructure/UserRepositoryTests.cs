using AgroSolutions.Domain.Entities;
using AgroSolutions.Infrastructure.Data;
using AgroSolutions.Infrastructure.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace AgroSolutions.Infrastructure.Tests;

public class UserRepositoryTests
{
    private static AgroSolutionsDbContext CreateContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<AgroSolutionsDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;
        return new AgroSolutionsDbContext(options);
    }

    [Fact]
    public async Task AddAndGetByEmail_Works()
    {
        var ctx = CreateContext(nameof(AddAndGetByEmail_Works));
        var repo = new AgroSolutions.Infrastructure.Data.Repositories.UserRepository(ctx);
        var user = new User("T", "u@u.com", BCrypt.Net.BCrypt.HashPassword("p"), "User");

        await repo.AddAsync(user);
        await ctx.SaveChangesAsync();

        var fetched = await repo.GetByEmailAsync(user.Email);
        Assert.NotNull(fetched);
        Assert.Equal(user.Email, fetched!.Email);
    }

    [Fact]
    public async Task ExistsByEmail_Count_Delete_Works()
    {
        var ctx = CreateContext(nameof(ExistsByEmail_Count_Delete_Works));
        var repo = new AgroSolutions.Infrastructure.Data.Repositories.UserRepository(ctx);
        var user = new User("X", "x@x.com", BCrypt.Net.BCrypt.HashPassword("p"), "User");

        await repo.AddAsync(user);
        await ctx.SaveChangesAsync();

        Assert.True(await repo.ExistsByEmailAsync(user.Email));
        Assert.Equal(1, await repo.CountAsync());

        var deleted = await repo.DeleteAsync(user.Id);
        await ctx.SaveChangesAsync();

        Assert.True(deleted);
        Assert.False(await repo.ExistsByEmailAsync(user.Email));
    }
}

