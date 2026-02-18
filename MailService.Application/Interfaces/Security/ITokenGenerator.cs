using MailService.Domain.Entities;

namespace MailService.Application.Interfaces.Security
{
    public interface ITokenGenerator
    {
        string Generate(User user);
    }
}
