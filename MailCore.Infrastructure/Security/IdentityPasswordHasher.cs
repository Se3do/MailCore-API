using MailCore.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace MailCore.Infrastructure.Security
{
    public sealed class IdentityPasswordHasher : IPasswordHasher
    {
        private static readonly PasswordHasher<object> _hasher = new();

        public string Hash(string password)
            => _hasher.HashPassword(null!, password);

        public bool Verify(string password, string passwordHash)
            => _hasher.VerifyHashedPassword(null!, passwordHash, password)
               == PasswordVerificationResult.Success;
    }
}
