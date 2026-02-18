namespace MailService.Domain.Security
{
    public static class PasswordHasher
    {
        private static readonly Microsoft.AspNetCore.Identity.PasswordHasher<object> _hasher
            = new();

        public static string Hash(string password)
            => _hasher.HashPassword(null!, password);

        public static bool Verify(string password, string hash)
            => _hasher.VerifyHashedPassword(null!, hash, password)
               == Microsoft.AspNetCore.Identity.PasswordVerificationResult.Success;
    }

}
