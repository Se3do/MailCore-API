using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace MailCore.Infrastructure.Data.Context
{
    public class MailCoreDbContextFactory : IDesignTimeDbContextFactory<MailCoreDbContext>
    {
        public MailCoreDbContext CreateDbContext(string[] args)
        {
            var options = new DbContextOptionsBuilder<MailCoreDbContext>()
                .UseSqlServer("Server=.;Database=MailService;TrustServerCertificate=True;MultipleActiveResultSets=true;Integrated Security=SSPI;")
                .Options;

            return new MailCoreDbContext(options);
        }
    }
}
