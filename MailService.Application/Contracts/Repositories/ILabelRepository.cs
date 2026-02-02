using MailService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MailService.Application.Contracts.Repositories
{
    public interface ILabelRepository
    {
        Task AddAsync(Label label);
        Task<Label?> GetByIdAsync(Guid Id);
        Task<List<Label>> GetAllAsync(Guid UserId);
        Task SaveChangesAsync();
    }
}
