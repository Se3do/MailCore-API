using System.Globalization;
using System.Text;
using MailService.Domain.Common;

namespace MailService.Application.Common.Pagination
{
    public static class CursorCodec
    {
        public static Cursor Decode(string? encoded)
        {
            if (string.IsNullOrWhiteSpace(encoded))
                return Cursor.Initial;

            try
            {
                var bytes = Convert.FromBase64String(encoded);
                var raw = Encoding.UTF8.GetString(bytes);
                var parts = raw.Split('|');

                if (parts.Length != 2)
                    return Cursor.Initial;

                if (!DateTime.TryParseExact(
                        parts[0],
                        "O",
                        CultureInfo.InvariantCulture,
                        DateTimeStyles.RoundtripKind,
                        out var timestamp))
                    return Cursor.Initial;

                if (!Guid.TryParse(parts[1], out var id))
                    return Cursor.Initial;

                return new Cursor(timestamp, id);
            }
            catch
            {
                return Cursor.Initial;
            }
        }

        public static string Encode(Cursor cursor)
        {
            var raw = $"{cursor.Timestamp:O}|{cursor.Id}";
            return Convert.ToBase64String(
                Encoding.UTF8.GetBytes(raw));
        }
    }
}
