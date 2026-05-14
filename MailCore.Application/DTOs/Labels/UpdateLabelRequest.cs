namespace MailCore.Application.DTOs.Labels;

/// <summary>Request to update an existing label's properties.</summary>
public sealed record UpdateLabelRequest(
    string Name,
    string? Color
);
