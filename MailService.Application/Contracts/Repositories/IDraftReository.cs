using MailService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MailService.Application.Contracts.Repositories
{
    public interface IDraftRepository
    {
        Task AddAsync(Draft draft);
        Task<Draft?> GetByIdAsync(Guid Id);
        Task<Draft> GetAllAsync(Guid UserId);
        Task SaveChangesAsync();
    }
}
