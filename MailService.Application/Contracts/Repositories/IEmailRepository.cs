using MailService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MailService.Application.Contracts.Repositories
{
    public interface IEmailRepository
    {
        Task AddAsync(Email email);
        Task<Email> GetByIdAsync(Guid Id);
        Task<List<Email>> GetInboxAsync(Guid UserId);
        Task SaveChangesAsync();
    }
}
