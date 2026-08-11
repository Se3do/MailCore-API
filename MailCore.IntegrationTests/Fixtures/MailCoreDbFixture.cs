using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Images;
using MailCore.Domain.Interfaces;
using MailCore.Infrastructure.Data.Context;
using MailCore.Infrastructure.Data.Seeding;
using MailCore.Infrastructure.Security;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Testcontainers.MsSql;

namespace MailCore.IntegrationTests.Fixtures;

public class MailCoreDbFixture : IAsyncLifetime
{
    private const string DatabaseName = "mailcore";

    private static readonly IFutureDockerImage SqlImage = new ImageFromDockerfileBuilder()
        .WithName("mailcore-sqlserver-fts:latest")
        .WithDockerfileDirectory(Path.Combine(
            CommonDirectoryPath.GetSolutionDirectory().DirectoryPath,
            "docker", "sqlserver-fts"))
        .Build();

    private readonly MsSqlContainer _container = new MsSqlBuilder(SqlImage)
        .Build();

    private string ConnectionString
    {
        get
        {
            var builder = new SqlConnectionStringBuilder(_container.GetConnectionString())
            {
                InitialCatalog = DatabaseName
            };
            return builder.ConnectionString;
        }
    }

    public async Task InitializeAsync()
    {
        await SqlImage.CreateAsync();
        await _container.StartAsync();
        await CreateDatabaseAsync();
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

    public async Task WaitForFullTextIndexAsync(CancellationToken cancellationToken = default)
    {
        using var connection = new SqlConnection(ConnectionString);
        await connection.OpenAsync(cancellationToken);
        for (var attempt = 0; attempt < 100; attempt++)
        {
            using var command = new SqlCommand("""
                SELECT OBJECTPROPERTY(OBJECT_ID('Emails'), 'TableFulltextPendingChanges')
                     + OBJECTPROPERTY(OBJECT_ID('Users'), 'TableFulltextPendingChanges')
                """, connection);
            if ((int)await command.ExecuteScalarAsync(cancellationToken) == 0)
            {
                return;
            }
            await Task.Delay(100, cancellationToken);
        }
        throw new TimeoutException("Full-text index did not catch up.");
    }

    private async Task CreateDatabaseAsync()
    {
        using var connection = new SqlConnection(_container.GetConnectionString());
        await connection.OpenAsync();
        using var command = new SqlCommand($"IF DB_ID('{DatabaseName}') IS NULL CREATE DATABASE [{DatabaseName}]", connection);
        await command.ExecuteNonQueryAsync();
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
