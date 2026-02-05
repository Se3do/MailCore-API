using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MailService.Domain.Interfaces
{
    public interface IUnitOfWork
    {
        public Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
