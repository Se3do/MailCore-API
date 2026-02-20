using MailService.Application.Commands.Drafts.CreateDraft;
using MailService.Application.Commands.Drafts.DeleteDraft;
using MailService.Application.Commands.Drafts.UpdateDraft;
using MailService.Application.Commands.Emails.ForwardEmail;
using MailService.Application.Commands.Emails.ReplyEmail;
using MailService.Application.Commands.Emails.SendEmail;
using MailService.Application.Commands.Labels.AssignLabel;
using MailService.Application.Commands.Labels.CreateLabel;
using MailService.Application.Commands.Labels.DeleteLabel;
using MailService.Application.Commands.Labels.UnassignLabel;
using MailService.Application.Commands.Labels.UpdateLabel;
using MailService.Application.Commands.Mailbox.MarkDeleted;
using MailService.Application.Commands.Mailbox.MarkRead;
using MailService.Application.Commands.Mailbox.MarkSpam;
using MailService.Application.Commands.Mailbox.MarkStarred;
using MailService.Application.Commands.Mailbox.MarkUnread;
using MailService.Application.Commands.Mailbox.Restore;
using MailService.Application.Commands.Mailbox.Unspam;
using MailService.Application.Commands.Mailbox.Unstar;
using MailService.Application.Emails;
using MailService.Application.Interfaces.Services;
using MailService.Application.Queries.Drafts.GetDraftById;
using MailService.Application.Queries.Drafts.GetDraftsPaged;
using MailService.Application.Queries.Email.GetSentById;
using MailService.Application.Queries.Email.GetSentPaged;
using MailService.Application.Queries.Labels.GetAllLabels;
using MailService.Application.Queries.Mailbox.GetByThreadPaged;
using MailService.Application.Queries.Mailbox.GetInboxPaged;
using MailService.Application.Queries.Mailbox.GetMailById;
using MailService.Application.Queries.Mailbox.GetSpamPaged;
using MailService.Application.Queries.Mailbox.GetStarredPaged;
using MailService.Application.Queries.Mailbox.GetTrashPaged;
using MailService.Application.Queries.Mailbox.GetUnreadPaged;
using MailService.Application.Services;
using MailService.Application.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace MailService.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationDI(this IServiceCollection services)
        {
            // ===================== FACADES =====================

            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IDraftService, DraftService>();
            services.AddScoped<IMailboxService, MailboxService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<ILabelService, LabelService>();

            // ===================== SHARED APPLICATION SERVICES =====================

            services.AddScoped<EmailComposer>();

            // ===================== EMAIL COMMANDS =====================

            services.AddScoped<SendEmailCommandHandler>();
            services.AddScoped<ReplyEmailCommandHandler>();
            services.AddScoped<ForwardEmailCommandHandler>();

            // ===================== EMAIL QUERIES =====================
            services.AddScoped<GetSentPagedQueryHandler>();
            services.AddScoped<GetSentByIdQueryHandler>();

            // ===================== DRAFT COMMANDS =====================

            services.AddScoped<CreateDraftCommandHandler>();
            services.AddScoped<UpdateDraftCommandHandler>();
            services.AddScoped<DeleteDraftCommandHandler>();

            // ===================== DRAFT QUERIES =====================

            services.AddScoped<GetDraftByIdQueryHandler>();
            services.AddScoped<GetDraftsPagedQueryHandler>();

            // ===================== MAILBOX COMMANDS =====================

            services.AddScoped<MarkMailReadCommandHandler>();
            services.AddScoped<MarkMailUnreadCommandHandler>();
            services.AddScoped<MarkMailSpamCommandHandler>();
            services.AddScoped<UnspamMailCommandHandler>();
            services.AddScoped<MarkMailStarredCommandHandler>();
            services.AddScoped<UnstarMailCommandHandler>();
            services.AddScoped<MarkMailDeletedCommandHandler>();
            services.AddScoped<RestoreMailCommandHandler>();

            // ===================== MAILBOX QUERIES =====================

            services.AddScoped<GetMailByIdQueryHandler>();
            services.AddScoped<GetInboxPagedQueryHandler>();
            services.AddScoped<GetUnreadPagedQueryHandler>();
            services.AddScoped<GetStarredPagedQueryHandler>();
            services.AddScoped<GetSpamPagedQueryHandler>();
            services.AddScoped<GetTrashPagedQueryHandler>();
            services.AddScoped<GetByThreadPagedQueryHandler>();

            // ===================== MAILBOX COMMANDS =====================

            services.AddScoped<CreateLabelCommandHandler>();
            services.AddScoped<UpdateLabelCommandHandler>();
            services.AddScoped<DeleteLabelCommandHandler>();
            services.AddScoped<AssignLabelCommandHandler>();
            services.AddScoped<UnassignLabelCommandHandler>();

            // ===================== MAILBOX QUERIES =====================

            services.AddScoped<GetAllLabelsQueryHandler>();

            return services;
        }
    }
}
