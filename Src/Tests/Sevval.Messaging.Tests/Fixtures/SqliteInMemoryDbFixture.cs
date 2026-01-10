using Microsoft.AspNetCore.Http;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Sevval.Persistence.Context;

namespace Sevval.Messaging.Tests.Fixtures;

public sealed class SqliteInMemoryDbFixture : IAsyncLifetime
{
    private readonly SqliteConnection _connection;

    public SqliteInMemoryDbFixture()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();
    }

    public ApplicationDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite(_connection)
            .Options;

        var accessor = new HttpContextAccessor { HttpContext = new DefaultHttpContext() };

        var context = new ApplicationDbContext(options, accessor);
        context.Database.EnsureCreated();
        return context;
    }

    public async Task ResetAsync()
    {
        await using var context = CreateContext();
        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public async Task DisposeAsync()
    {
        await _connection.DisposeAsync();
    }
}
