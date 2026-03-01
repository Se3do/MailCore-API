namespace MailCore.Application.DTOs.Labels;
public sealed record LabelDto(
    Guid Id,
    string Name,
    string? Color
);
