using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace MailCore.Infrastructure.Data.Context
{
    public class MailCoreDbContextFactory : IDesignTimeDbContextFactory<MailCoreDbContext>
    {
        public MailCoreDbContext CreateDbContext(string[] args)
        {
            var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")
                ?? "Server=.;Database=MailService;TrustServerCertificate=True;MultipleActiveResultSets=true;Integrated Security=SSPI;";

            var options = new DbContextOptionsBuilder<MailCoreDbContext>()
                .UseSqlServer(connectionString)
                .Options;

            return new MailCoreDbContext(options);
        }
    }
}
