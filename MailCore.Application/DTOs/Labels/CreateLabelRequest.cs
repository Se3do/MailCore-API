namespace MailCore.Application.DTOs.Labels;

/// <summary>Request to create a new label.</summary>
public sealed record CreateLabelRequest(
    string Name,
    string? Color
);
