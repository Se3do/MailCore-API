namespace MailCore.Application.DTOs.Labels;

public sealed record CreateLabelRequest(
    string Name,
    string? Color
);
