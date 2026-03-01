using MailCore.Domain.Entities;

namespace MailCore.Application.Interfaces.Security
{
    public interface ITokenGenerator
    {
        string Generate(User user);
    }
}
