using System.Security.Claims;

namespace MailCore.API.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static Guid GetUserId(this ClaimsPrincipal user)
        {
            var value = user.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(value))
                throw new UnauthorizedAccessException("UserId claim is missing.");

            return Guid.Parse(value);
        }
    }
}
