using MailCore.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace MailCore.Infrastructure.Tests;

/// <summary>
/// Provides a fresh in-memory <see cref="MailCoreDbContext"/> per test to guarantee isolation.
/// </summary>
public abstract class RepositoryTestBase : IDisposable
{
    protected readonly MailCoreDbContext Context;

    protected RepositoryTestBase()
    {
        var options = new DbContextOptionsBuilder<MailCoreDbContext>()
   .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
  .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        Context = new MailCoreDbContext(options);
    }

    /// <summary>
    /// Persists pending changes and clears the change tracker so subsequent reads
    /// hit the in-memory store (simulating a new DbContext scope).
    /// </summary>
    protected async Task SaveAndDetachAsync()
    {
        await Context.SaveChangesAsync();
        Context.ChangeTracker.Clear();
    }

    public void Dispose()
    {
        Context.Dispose();
        GC.SuppressFinalize(this);
    }
}
