using MailService.Domain.Interfaces;
using MailService.Infrastructure.Data.Context;

namespace MailService.Infrastructure.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly MailServiceDbContext _context;

        public UnitOfWork(MailServiceDbContext context)
        {
            _context = context;
        }

        public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return _context.SaveChangesAsync(cancellationToken);
        }
    }
}
