using MailCore.Domain.Interfaces;
using MailCore.Infrastructure.Data.Context;

namespace MailCore.Infrastructure.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly MailCoreDbContext _context;

        public UnitOfWork(MailCoreDbContext context)
        {
            _context = context;
        }

        public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return _context.SaveChangesAsync(cancellationToken);
        }
    }
}
