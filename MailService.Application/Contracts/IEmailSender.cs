using MailService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MailService.Application.Contracts
{
    public interface IEmailSender
    {
        Task SendAsync(Email email);
    }
}
