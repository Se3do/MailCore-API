using MailService.Application.Common.Pagination;
using MailService.Application.Queries.Mailbox.GetByLabelPaged;
using MailService.Application.Queries.Mailbox.GetByThreadPaged;
using MailService.Application.Queries.Mailbox.GetInboxPaged;
using MailService.Application.Queries.Mailbox.GetMailById;
using MailService.Application.Queries.Mailbox.GetSpamPaged;
using MailService.Application.Queries.Mailbox.GetStarredPaged;
using MailService.Application.Queries.Mailbox.GetTrashPaged;
using MailService.Application.Queries.Mailbox.GetUnreadPaged;
using MailService.Application.Commands.Mailbox.MarkRead;
using MailService.Application.Commands.Mailbox.MarkUnread;
using MailService.Application.Commands.Mailbox.MarkSpam;
using MailService.Application.Commands.Mailbox.Unspam;
using MailService.Application.Commands.Mailbox.MarkStarred;
using MailService.Application.Commands.Mailbox.Unstar;
using MailService.Application.Commands.Mailbox.MarkDeleted;
using MailService.Application.Commands.Mailbox.Restore;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MailService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MailboxController : ControllerBase
    {
        private readonly IMediator _mediator;

        public MailboxController(IMediator mediator)
        {
            _mediator = mediator;
        }

        private Guid UserId =>
            Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        // ===================== READ =====================

        [HttpGet("inbox")]
        public async Task<IActionResult> GetInbox([FromQuery] CursorPaginationQuery query, CancellationToken ct)
        {
            var result = await _mediator.Send(new GetInboxPagedQuery(UserId, query), ct);
            return Ok(result);
        }

        [HttpGet("unread")]
        public async Task<IActionResult> GetUnread([FromQuery] CursorPaginationQuery query, CancellationToken ct)
        {
            var result = await _mediator.Send(new GetUnreadPagedQuery(UserId, query), ct);
            return Ok(result);
        }

        [HttpGet("starred")]
        public async Task<IActionResult> GetStarred([FromQuery] CursorPaginationQuery query, CancellationToken ct)
        {
            var result = await _mediator.Send(new GetStarredPagedQuery(UserId, query), ct);
            return Ok(result);
        }

        [HttpGet("spam")]
        public async Task<IActionResult> GetSpam([FromQuery] CursorPaginationQuery query, CancellationToken ct)
        {
            var result = await _mediator.Send(new GetSpamPagedQuery(UserId, query), ct);
            return Ok(result);
        }

        [HttpGet("trash")]
        public async Task<IActionResult> GetTrash([FromQuery] CursorPaginationQuery query, CancellationToken ct)
        {
            var result = await _mediator.Send(new GetTrashPagedQuery(UserId, query), ct);
            return Ok(result);
        }

        [HttpGet("thread/{threadId}")]
        public async Task<IActionResult> GetByThread(Guid threadId, [FromQuery] CursorPaginationQuery query, CancellationToken ct)
        {
            var result = await _mediator.Send(new GetByThreadPagedQuery(UserId, threadId, query), ct);
            return Ok(result);
        }

        [HttpGet("label/{labelId}")]
        public async Task<IActionResult> GetByLabel(Guid labelId, [FromQuery] CursorPaginationQuery query, CancellationToken ct)
        {
            var result = await _mediator.Send(new GetByLabelPagedQuery(UserId, labelId, query), ct);
            return Ok(result);
        }

        [HttpGet("{mailRecipientId}")]
        public async Task<IActionResult> GetById(Guid mailRecipientId, CancellationToken ct)
        {
            var result = await _mediator.Send(new GetMailByIdQuery(UserId, mailRecipientId), ct);
            return result is null ? NotFound() : Ok(result);
        }

        // ===================== WRITE =====================

        [HttpPost("{mailRecipientId}/read")]
        public async Task<IActionResult> MarkRead(Guid mailRecipientId, CancellationToken ct)
        {
            var result = await _mediator.Send(new MarkMailReadCommand(UserId, mailRecipientId), ct);
            return result ? NoContent() : NotFound();
        }

        [HttpPost("{mailRecipientId}/unread")]
        public async Task<IActionResult> MarkUnread(Guid mailRecipientId, CancellationToken ct)
        {
            var result = await _mediator.Send(new MarkMailUnreadCommand(UserId, mailRecipientId), ct);
            return result ? NoContent() : NotFound();
        }

        [HttpPost("{mailRecipientId}/spam")]
        public async Task<IActionResult> MarkSpam(Guid mailRecipientId, CancellationToken ct)
        {
            var result = await _mediator.Send(new MarkMailSpamCommand(UserId, mailRecipientId), ct);
            return result ? NoContent() : NotFound();
        }

        [HttpPost("{mailRecipientId}/unspam")]
        public async Task<IActionResult> Unspam(Guid mailRecipientId, CancellationToken ct)
        {
            var result = await _mediator.Send(new UnspamMailCommand(UserId, mailRecipientId), ct);
            return result ? NoContent() : NotFound();
        }

        [HttpPost("{mailRecipientId}/star")]
        public async Task<IActionResult> Star(Guid mailRecipientId, CancellationToken ct)
        {
            var result = await _mediator.Send(new MarkMailStarredCommand(UserId, mailRecipientId), ct);
            return result ? NoContent() : NotFound();
        }

        [HttpPost("{mailRecipientId}/unstar")]
        public async Task<IActionResult> Unstar(Guid mailRecipientId, CancellationToken ct)
        {
            var result = await _mediator.Send(new UnstarMailCommand(UserId, mailRecipientId), ct);
            return result ? NoContent() : NotFound();
        }

        [HttpDelete("{mailRecipientId}")]
        public async Task<IActionResult> Delete(Guid mailRecipientId, CancellationToken ct)
        {
            var result = await _mediator.Send(new MarkMailDeletedCommand(UserId, mailRecipientId), ct);
            return result ? NoContent() : NotFound();
        }

        [HttpPost("{mailRecipientId}/restore")]
        public async Task<IActionResult> Restore(Guid mailRecipientId, CancellationToken ct)
        {
            var result = await _mediator.Send(new RestoreMailCommand(UserId, mailRecipientId), ct);
            return result ? NoContent() : NotFound();
        }
    }
}
