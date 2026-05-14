using MailCore.Domain.Interfaces;
using MailCore.Infrastructure.Data.Context;
using MailCore.Infrastructure.Data.Seeding;
using MailCore.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using Testcontainers.MsSql;

namespace MailCore.IntegrationTests.Fixtures;

public class MailCoreDbFixture : IAsyncLifetime
{
    private readonly MsSqlContainer _container = new MsSqlBuilder("mcr.microsoft.com/mssql/server:2022-latest")
        .Build();

    public string ConnectionString => _container.GetConnectionString();

    public async Task InitializeAsync()
    {
        await _container.StartAsync();
        await RunMigrationsAsync();
        await SeedAsync();
    }

    public async Task DisposeAsync()
    {
        await _container.DisposeAsync();
    }

    public MailCoreDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<MailCoreDbContext>()
            .UseSqlServer(ConnectionString)
            .Options;

        return new MailCoreDbContext(options);
    }

    private async Task RunMigrationsAsync()
    {
        using var context = CreateContext();
        await context.Database.MigrateAsync();
    }

    private async Task SeedAsync()
    {
        using var context = CreateContext();
        IPasswordHasher passwordHasher = new IdentityPasswordHasher();
        await DbSeeder.SeedAsync(context, passwordHasher);
    }
}
