using Asp.Versioning;
using MailCore.API.Extensions;
using MailCore.Application.Commands.Emails.SendEmail;
using MailCore.Application.Commands.Emails.ForwardEmail;
using MailCore.Application.Commands.Emails.ReplyEmail;
using MailCore.Application.Queries.Email.GetSentPaged;
using MailCore.Application.Queries.Email.GetSentById;
using MailCore.Application.Queries.Email.GetDeliveryStatus;
using MailCore.Application.Queries.Email.SearchEmails;
using MailCore.Application.Common.Pagination;
using MailCore.Application.DTOs.Emails;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MailCore.API.Controllers
{
    /// <summary>Handles email operations including sending, forwarding, replying, and searching sent emails.</summary>
    [Authorize]
    [Route("api/v{version:apiVersion}/mail")]
    [ApiController]
    [ApiVersion("1.0")]
    public class MailController : ControllerBase
    {
        private readonly IMediator _mediator;

        public MailController(IMediator mediator)
        {
            _mediator = mediator;
        }
        private Guid UserId =>
            User.GetUserId();

        /// <summary>Sends a new email with optional attachments.</summary>
        /// <param name="request">The email details including recipients, subject, body, and attachments.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>No content.</returns>
        [HttpPost("send")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SendEmail([FromForm] SendEmailRequest request, CancellationToken ct)
        {
            await _mediator.Send(new SendEmailCommand(UserId, request), ct);
            return NoContent();
        }

        /// <summary>Forwards an existing email to new recipients.</summary>
        /// <param name="emailId">The ID of the email to forward.</param>
        /// <param name="request">The forward details including recipients and additional body text.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>No content.</returns>
        [HttpPost("{emailId}/forward")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ForwardEmail(Guid emailId, [FromForm] ForwardEmailRequest request, CancellationToken ct)
        {
            await _mediator.Send(new ForwardEmailCommand(UserId, emailId, request), ct);
            return NoContent();
        }

        /// <summary>Replies to an existing email.</summary>
        /// <param name="emailId">The ID of the email to reply to.</param>
        /// <param name="request">The reply details including body text.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>No content.</returns>
        [HttpPost("{emailId}/reply")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ReplyEmail(Guid emailId, [FromForm] ReplyEmailRequest request, CancellationToken ct)
        {
            await _mediator.Send(new ReplyEmailCommand(UserId, emailId, request), ct);
            return NoContent();
        }

        /// <summary>Gets a paginated list of sent emails for the current user.</summary>
        /// <param name="query">Pagination cursor parameters.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>A cursor-paginated result of <see cref="EmailSummaryDto"/>.</returns>
        [HttpGet("sent")]
        [ProducesResponseType(typeof(CursorPagedResult<EmailSummaryDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetSent([FromQuery] CursorPaginationQuery query, CancellationToken ct)
        {
            var result = await _mediator.Send(new GetSentPagedQuery(UserId, query), ct);
            return Ok(result);
        }

        /// <summary>Gets the full details of a sent email.</summary>
        /// <param name="emailId">The ID of the sent email.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>An <see cref="EmailDto"/> with full email details.</returns>
        [HttpGet("sent/{emailId}")]
        [ProducesResponseType(typeof(EmailDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetSentDetail(Guid emailId, CancellationToken ct)
        {
            var result = await _mediator.Send(new GetSentByIdQuery(UserId, emailId), ct);
            return Ok(result);
        }

        /// <summary>Gets the delivery status of a sent email.</summary>
        /// <param name="emailId">The ID of the sent email.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>A <see cref="DeliveryStatusDto"/> with delivery information.</returns>
        [HttpGet("{emailId}/status")]
        [ProducesResponseType(typeof(DeliveryStatusDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetDeliveryStatus(Guid emailId, CancellationToken ct)
        {
            var result = await _mediator.Send(new GetDeliveryStatusQuery(UserId, emailId), ct);
            return Ok(result);
        }

        /// <summary>Searches sent emails by query string.</summary>
        /// <param name="q">The search query.</param>
        /// <param name="pagination">Pagination cursor parameters.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>A cursor-paginated result of matching <see cref="EmailSummaryDto"/>.</returns>
        [HttpGet("search")]
        [ProducesResponseType(typeof(CursorPagedResult<EmailSummaryDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> SearchEmails([FromQuery] string q, [FromQuery] CursorPaginationQuery pagination, CancellationToken ct)
        {
            var result = await _mediator.Send(new SearchEmailsQuery(UserId, q, pagination), ct);
            return Ok(result);
        }
    }
}
