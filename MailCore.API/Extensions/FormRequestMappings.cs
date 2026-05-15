using MailCore.API.Contracts.Requests;
using MailCore.Application.DTOs.Emails;

namespace MailCore.API.Extensions;

public static class FormRequestMappings
{
    public static SendEmailRequest ToApplicationRequest(this SendEmailFormRequest form)
    {
        return new SendEmailRequest(
            form.Subject,
            form.Body,
            form.To,
            form.Cc,
            form.Bcc,
            form.ThreadId,
            form.Attachments.ToFileDataList());
    }

    public static ForwardEmailRequest ToApplicationRequest(this ForwardEmailFormRequest form)
    {
        return new ForwardEmailRequest(
            form.Body,
            form.To,
            form.Cc,
            form.Bcc,
            form.Attachments.ToFileDataList());
    }

    public static ReplyEmailRequest ToApplicationRequest(this ReplyEmailFormRequest form)
    {
        return new ReplyEmailRequest(
            form.Body,
            form.To,
            form.Cc,
            form.Bcc,
            form.Attachments.ToFileDataList());
    }
}
