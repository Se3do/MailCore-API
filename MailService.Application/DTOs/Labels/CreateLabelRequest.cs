namespace MailService.Application.DTOs.Labels;

public sealed record CreateLabelRequest(
    string Name,
    string? Color
);
