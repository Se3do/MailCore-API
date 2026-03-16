using System.Text.Json;

namespace MailCore.Application.Common.Drafts;

public static class DraftRecipientsCodec
{
    public static string? Serialize(IReadOnlyList<string>? recipients)
    {
        var normalized = Normalize(recipients);
        return normalized.Count == 0 ? null : JsonSerializer.Serialize(normalized);
    }

    public static IReadOnlyList<string> Deserialize(string? payload)
    {
        if (string.IsNullOrWhiteSpace(payload))
            return [];

        try
        {
            var values = JsonSerializer.Deserialize<List<string>>(payload);
            return Normalize(values);
        }
        catch
        {
            // Backward compatibility in case legacy delimiter-based values exist.
            var split = payload.Split([';', ','], StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            return Normalize(split);
        }
    }

    private static IReadOnlyList<string> Normalize(IEnumerable<string>? recipients)
    {
        if (recipients is null)
            return [];

        return recipients
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Select(x => x.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
    }
}
