namespace MailService.Application.DTOs.Email
{
    public record SendResultDto(bool Success, string? ErrorMessage = null, Guid? EmailId = null);
}