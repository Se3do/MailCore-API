using MailService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MailService.Domain.Interfaces
{
    public interface ILabelRepository
    {
        Task AddAsync(Label label, CancellationToken cancellationToken = default);
        Task<Label> UpdateAsync(Guid Id, Label label, CancellationToken cancellationToken = default);
        Task DeleteAsync(Guid Id, CancellationToken cancellationToken = default);
        Task<Label?> GetByIdAsync(Guid Id, CancellationToken cancellationToken = default);
        Task<List<Label>> GetAllAsync(Guid UserId, CancellationToken cancellationToken = default);
    }
}
