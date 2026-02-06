namespace MailService.Application.DTOs.Labels;

public sealed record UpdateLabelRequest(
    string Name,
    string? Color
);
