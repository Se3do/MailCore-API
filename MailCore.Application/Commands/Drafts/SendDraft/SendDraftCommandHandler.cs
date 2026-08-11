using MailCore.Application.Common.Drafts;
using MailCore.Application.Emails;
using MailCore.Application.Exceptions;
using MailCore.Domain.Entities;
using MailCore.Domain.Enums;
using MailCore.Domain.Interfaces;
using MediatR;

namespace MailCore.Application.Commands.Drafts.SendDraft;

public sealed class SendDraftCommandHandler : IRequestHandler<SendDraftCommand>
{
    private readonly IDraftRepository _draftRepository;
    private readonly IEmailRepository _emailRepository;
    private readonly IUserRepository _userRepository;
    private readonly EmailComposer _emailComposer;

    public SendDraftCommandHandler(
        IDraftRepository draftRepository,
        IEmailRepository emailRepository,
        IUserRepository userRepository,
        EmailComposer emailComposer)
    {
        _draftRepository = draftRepository;
        _emailRepository = emailRepository;
        _userRepository = userRepository;
        _emailComposer = emailComposer;
    }

    public async Task Handle(SendDraftCommand command, CancellationToken ct)
    {
        var draft = await _draftRepository.GetByIdAsync(command.DraftId, ct)
            ?? throw new NotFoundException($"Draft {command.DraftId} not found.");

        if (draft.UserId != command.UserId)
            throw new ForbiddenException("You do not have access to this draft.");

        if (string.IsNullOrWhiteSpace(draft.Body))
            throw new ValidationException("Draft body cannot be empty.");

        var toRecipients = DraftRecipientsCodec.Deserialize(draft.ToRecipients);
        if (toRecipients.Count == 0)
            throw new ValidationException("Draft must include at least one recipient in To.");

        var sender = await _userRepository.GetByIdAsync(command.UserId, ct)
            ?? throw new NotFoundException("Sender not found.");

        var now = DateTime.UtcNow;

        var thread = await _emailComposer.GetOrCreateThreadAsync(draft.ThreadId, now, ct);

        var subject = string.IsNullOrWhiteSpace(draft.Subject) ? "(No subject)" : draft.Subject;
        var email = Email.Create(command.UserId, subject, draft.Body, thread.Id);

        await _emailRepository.AddAsync(email, ct);

        var ccRecipients = DraftRecipientsCodec.Deserialize(draft.CcRecipients);
        var bccRecipients = DraftRecipientsCodec.Deserialize(draft.BccRecipients);

        await _emailComposer.AddRecipientsAsync(email, toRecipients, ccRecipients, bccRecipients, now, ct);

        // Delete draft after sending; it has been promoted to an email.
        await _draftRepository.DeleteAsync(draft.Id, ct);
    }
}
