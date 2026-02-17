using System.Text;

namespace MailService.Domain.Common
{
    public sealed record Cursor(DateTime Timestamp, Guid Id)
    {
        public static Cursor Deserialize(string? encoded)
        {
            if (string.IsNullOrWhiteSpace(encoded))
                return new Cursor(DateTime.MaxValue, Guid.Empty);

            var bytes = Convert.FromBase64String(encoded);
            var parts = Encoding.UTF8.GetString(bytes).Split('|');

            return new Cursor(
                DateTime.Parse(parts[0]),
                Guid.Parse(parts[1]));
        }

        public string Serialize()
        {
            var raw = $"{Timestamp:O}|{Id}";
            return Convert.ToBase64String(
                Encoding.UTF8.GetBytes(raw));
        }
    }
}
