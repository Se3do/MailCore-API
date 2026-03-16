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
    private readonly IThreadRepository _threadRepository;
    private readonly IUserRepository _userRepository;
    private readonly EmailComposer _emailComposer;

    public SendDraftCommandHandler(
        IDraftRepository draftRepository,
        IEmailRepository emailRepository,
        IThreadRepository threadRepository,
        IUserRepository userRepository,
        EmailComposer emailComposer)
    {
        _draftRepository = draftRepository;
        _emailRepository = emailRepository;
        _threadRepository = threadRepository;
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

        var thread = await GetOrCreateThreadAsync(draft.ThreadId, now, ct);

        var email = new Email
        {
            Id = Guid.NewGuid(),
            SenderId = command.UserId,
            Subject = string.IsNullOrWhiteSpace(draft.Subject) ? "(No subject)" : draft.Subject,
            Body = draft.Body,
            CreatedAt = now,
            ThreadId = thread.Id
        };

        await _emailRepository.AddAsync(email, ct);

        await _emailComposer.AddRecipientsAsync(email, toRecipients, RecipientType.To, now, ct);

        var ccRecipients = DraftRecipientsCodec.Deserialize(draft.CcRecipients);
        if (ccRecipients.Count > 0)
            await _emailComposer.AddRecipientsAsync(email, ccRecipients, RecipientType.Cc, now, ct);

        var bccRecipients = DraftRecipientsCodec.Deserialize(draft.BccRecipients);
        if (bccRecipients.Count > 0)
            await _emailComposer.AddRecipientsAsync(email, bccRecipients, RecipientType.Bcc, now, ct);

        // Delete draft after sending; it has been promoted to an email.
        await _draftRepository.DeleteAsync(draft.Id, ct);
    }

    private async Task<Domain.Entities.Thread> GetOrCreateThreadAsync(Guid? threadId, DateTime now, CancellationToken ct)
    {
        if (threadId.HasValue)
        {
            var existing = await _threadRepository.GetByIdAsync(threadId.Value, ct)
                ?? throw new NotFoundException($"Thread {threadId.Value} not found.");
            existing.LastMessageAt = now;
            return existing;
        }

        var thread = new Domain.Entities.Thread
        {
            Id = Guid.NewGuid(),
            CreatedAt = now,
            LastMessageAt = now
        };

        await _threadRepository.AddAsync(thread, ct);
        return thread;
    }
}
