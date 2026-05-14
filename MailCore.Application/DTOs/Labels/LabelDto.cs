namespace MailCore.Application.DTOs.Labels;
/// <summary>A user-defined label for organizing emails.</summary>
public sealed record LabelDto(
    Guid Id,
    string Name,
    string? Color
);
