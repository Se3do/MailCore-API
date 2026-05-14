using Asp.Versioning;
using MailCore.API.Extensions;
using MailCore.Application.DTOs.Mailbox;
using MailCore.Application.Common.Pagination;
using MailCore.Application.Queries.Mailbox.GetByLabelPaged;
using MailCore.Application.Queries.Mailbox.GetByThreadPaged;
using MailCore.Application.Queries.Mailbox.GetInboxPaged;
using MailCore.Application.Queries.Mailbox.GetMailById;
using MailCore.Application.Queries.Mailbox.GetSpamPaged;
using MailCore.Application.Queries.Mailbox.GetStarredPaged;
using MailCore.Application.Queries.Mailbox.GetTrashPaged;
using MailCore.Application.Queries.Mailbox.GetUnreadPaged;
using MailCore.Application.Commands.Mailbox.MarkRead;
using MailCore.Application.Commands.Mailbox.MarkUnread;
using MailCore.Application.Commands.Mailbox.MarkSpam;
using MailCore.Application.Commands.Mailbox.Unspam;
using MailCore.Application.Commands.Mailbox.MarkStarred;
using MailCore.Application.Commands.Mailbox.Unstar;
using MailCore.Application.Commands.Mailbox.MarkDeleted;
using MailCore.Application.Commands.Mailbox.Restore;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MailCore.API.Controllers
{
    /// <summary>Handles mailbox operations for reading, organizing, and managing received emails.</summary>
    [Route("api/v{version:apiVersion}/mailbox")]
    [ApiController]
    [ApiVersion("1.0")]
    [Authorize]
    public class MailboxController : ControllerBase
    {
        private readonly IMediator _mediator;

        public MailboxController(IMediator mediator)
        {
            _mediator = mediator;
        }

        private Guid UserId =>
            User.GetUserId();

        // ===================== READ =====================

        /// <summary>Gets paginated inbox emails.</summary>
        /// <param name="query">Pagination cursor parameters.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>A cursor-paginated result of <see cref="MailboxItemDto"/>.</returns>
        [HttpGet("inbox")]
        [ProducesResponseType(typeof(CursorPagedResult<MailboxItemDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetInbox([FromQuery] CursorPaginationQuery query, CancellationToken ct)
        {
            var result = await _mediator.Send(new GetInboxPagedQuery(UserId, query), ct);
            return Ok(result);
        }

        /// <summary>Gets paginated unread emails.</summary>
        /// <param name="query">Pagination cursor parameters.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>A cursor-paginated result of unread <see cref="MailboxItemDto"/>.</returns>
        [HttpGet("unread")]
        [ProducesResponseType(typeof(CursorPagedResult<MailboxItemDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetUnread([FromQuery] CursorPaginationQuery query, CancellationToken ct)
        {
            var result = await _mediator.Send(new GetUnreadPagedQuery(UserId, query), ct);
            return Ok(result);
        }

        /// <summary>Gets paginated starred emails.</summary>
        /// <param name="query">Pagination cursor parameters.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>A cursor-paginated result of starred <see cref="MailboxItemDto"/>.</returns>
        [HttpGet("starred")]
        [ProducesResponseType(typeof(CursorPagedResult<MailboxItemDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetStarred([FromQuery] CursorPaginationQuery query, CancellationToken ct)
        {
            var result = await _mediator.Send(new GetStarredPagedQuery(UserId, query), ct);
            return Ok(result);
        }

        /// <summary>Gets paginated spam emails.</summary>
        /// <param name="query">Pagination cursor parameters.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>A cursor-paginated result of spam <see cref="MailboxItemDto"/>.</returns>
        [HttpGet("spam")]
        [ProducesResponseType(typeof(CursorPagedResult<MailboxItemDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetSpam([FromQuery] CursorPaginationQuery query, CancellationToken ct)
        {
            var result = await _mediator.Send(new GetSpamPagedQuery(UserId, query), ct);
            return Ok(result);
        }

        /// <summary>Gets paginated trashed emails.</summary>
        /// <param name="query">Pagination cursor parameters.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>A cursor-paginated result of trashed <see cref="MailboxItemDto"/>.</returns>
        [HttpGet("trash")]
        [ProducesResponseType(typeof(CursorPagedResult<MailboxItemDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetTrash([FromQuery] CursorPaginationQuery query, CancellationToken ct)
        {
            var result = await _mediator.Send(new GetTrashPagedQuery(UserId, query), ct);
            return Ok(result);
        }

        /// <summary>Gets paginated emails belonging to a specific thread.</summary>
        /// <param name="threadId">The thread ID.</param>
        /// <param name="query">Pagination cursor parameters.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>A cursor-paginated result of <see cref="MailboxItemDto"/> for the thread.</returns>
        [HttpGet("thread/{threadId}")]
        [ProducesResponseType(typeof(CursorPagedResult<MailboxItemDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetByThread(Guid threadId, [FromQuery] CursorPaginationQuery query, CancellationToken ct)
        {
            var result = await _mediator.Send(new GetByThreadPagedQuery(UserId, threadId, query), ct);
            return Ok(result);
        }

        /// <summary>Gets paginated emails assigned to a specific label.</summary>
        /// <param name="labelId">The label ID.</param>
        /// <param name="query">Pagination cursor parameters.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>A cursor-paginated result of <see cref="MailboxItemDto"/> for the label.</returns>
        [HttpGet("label/{labelId}")]
        [ProducesResponseType(typeof(CursorPagedResult<MailboxItemDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetByLabel(Guid labelId, [FromQuery] CursorPaginationQuery query, CancellationToken ct)
        {
            var result = await _mediator.Send(new GetByLabelPagedQuery(UserId, labelId, query), ct);
            return Ok(result);
        }

        /// <summary>Gets the full details of a specific email in the mailbox.</summary>
        /// <param name="mailRecipientId">The mail recipient ID.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>A <see cref="MailboxDetailDto"/> with full email and metadata details.</returns>
        [HttpGet("{mailRecipientId}")]
        [ProducesResponseType(typeof(MailboxDetailDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetById(Guid mailRecipientId, CancellationToken ct)
        {
            var result = await _mediator.Send(new GetMailByIdQuery(UserId, mailRecipientId), ct);
            return Ok(result);
        }

        // ===================== WRITE =====================

        /// <summary>Marks an email as read.</summary>
        /// <param name="mailRecipientId">The mail recipient ID.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>No content.</returns>
        [HttpPost("{mailRecipientId}/read")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> MarkRead(Guid mailRecipientId, CancellationToken ct)
        {
            await _mediator.Send(new MarkMailReadCommand(UserId, mailRecipientId), ct);
            return NoContent();
        }

        /// <summary>Marks an email as unread.</summary>
        /// <param name="mailRecipientId">The mail recipient ID.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>No content.</returns>
        [HttpPost("{mailRecipientId}/unread")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> MarkUnread(Guid mailRecipientId, CancellationToken ct)
        {
            await _mediator.Send(new MarkMailUnreadCommand(UserId, mailRecipientId), ct);
            return NoContent();
        }

        /// <summary>Marks an email as spam.</summary>
        /// <param name="mailRecipientId">The mail recipient ID.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>No content.</returns>
        [HttpPost("{mailRecipientId}/spam")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> MarkSpam(Guid mailRecipientId, CancellationToken ct)
        {
            await _mediator.Send(new MarkMailSpamCommand(UserId, mailRecipientId), ct);
            return NoContent();
        }

        /// <summary>Removes an email from spam (marks as not spam).</summary>
        /// <param name="mailRecipientId">The mail recipient ID.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>No content.</returns>
        [HttpPost("{mailRecipientId}/unspam")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Unspam(Guid mailRecipientId, CancellationToken ct)
        {
            await _mediator.Send(new UnspamMailCommand(UserId, mailRecipientId), ct);
            return NoContent();
        }

        /// <summary>Stars (bookmarks) an email.</summary>
        /// <param name="mailRecipientId">The mail recipient ID.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>No content.</returns>
        [HttpPost("{mailRecipientId}/star")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Star(Guid mailRecipientId, CancellationToken ct)
        {
            await _mediator.Send(new MarkMailStarredCommand(UserId, mailRecipientId), ct);
            return NoContent();
        }

        /// <summary>Removes the star from an email.</summary>
        /// <param name="mailRecipientId">The mail recipient ID.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>No content.</returns>
        [HttpPost("{mailRecipientId}/unstar")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Unstar(Guid mailRecipientId, CancellationToken ct)
        {
            await _mediator.Send(new UnstarMailCommand(UserId, mailRecipientId), ct);
            return NoContent();
        }

        /// <summary>Moves an email to trash (soft delete).</summary>
        /// <param name="mailRecipientId">The mail recipient ID.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>No content.</returns>
        [HttpDelete("{mailRecipientId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Delete(Guid mailRecipientId, CancellationToken ct)
        {
            await _mediator.Send(new MarkMailDeletedCommand(UserId, mailRecipientId), ct);
            return NoContent();
        }

        /// <summary>Restores an email from trash back to the inbox.</summary>
        /// <param name="mailRecipientId">The mail recipient ID.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>No content.</returns>
        [HttpPost("{mailRecipientId}/restore")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Restore(Guid mailRecipientId, CancellationToken ct)
        {
            await _mediator.Send(new RestoreMailCommand(UserId, mailRecipientId), ct);
            return NoContent();
        }
    }
}
