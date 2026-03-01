namespace MailCore.Application.DTOs.Labels;

public sealed record UpdateLabelRequest(
    string Name,
    string? Color
);
