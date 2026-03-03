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

    public SendDraftCommandHandler(
        IDraftRepository draftRepository,
        IEmailRepository emailRepository,
        IThreadRepository threadRepository,
        IUserRepository userRepository)
    {
        _draftRepository = draftRepository;
        _emailRepository = emailRepository;
        _threadRepository = threadRepository;
        _userRepository = userRepository;
    }

    public async Task Handle(SendDraftCommand command, CancellationToken ct)
    {
        var draft = await _draftRepository.GetByIdAsync(command.DraftId, ct)
            ?? throw new NotFoundException($"Draft {command.DraftId} not found.");

        if (draft.UserId != command.UserId)
            throw new ForbiddenException("You do not have access to this draft.");

        if (string.IsNullOrWhiteSpace(draft.Body))
            throw new ValidationException("Draft body cannot be empty.");

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

        // Delete draft after sending — it has been promoted to an email
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
