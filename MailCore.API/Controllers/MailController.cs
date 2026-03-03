using Asp.Versioning;
using MailCore.API.Extensions;
using MailCore.Application.Commands.Emails.SendEmail;
using MailCore.Application.Commands.Emails.ForwardEmail;
using MailCore.Application.Commands.Emails.ReplyEmail;
using MailCore.Application.Queries.Email.GetSentPaged;
using MailCore.Application.Queries.Email.GetSentById;
using MailCore.Application.Common.Pagination;
using MailCore.Application.DTOs.Emails;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MailCore.API.Controllers
{
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

        [HttpPost("send")]
        public async Task<IActionResult> SendEmail([FromForm] SendEmailRequest request, CancellationToken ct)
        {
            await _mediator.Send(new SendEmailCommand(UserId, request), ct);
            return NoContent();
        }

        [HttpPost("{emailId}/forward")]
        public async Task<IActionResult> ForwardEmail(Guid emailId, [FromForm] ForwardEmailRequest request, CancellationToken ct)
        {
            await _mediator.Send(new ForwardEmailCommand(UserId, emailId, request), ct);
            return NoContent();
        }

        [HttpPost("{emailId}/reply")]
        public async Task<IActionResult> ReplyEmail(Guid emailId, [FromForm] ReplyEmailRequest request, CancellationToken ct)
        {
            await _mediator.Send(new ReplyEmailCommand(UserId, emailId, request), ct);
            return NoContent();
        }

        [HttpGet("sent")]
        public async Task<IActionResult> GetSent([FromQuery] CursorPaginationQuery query, CancellationToken ct)
        {
            var result = await _mediator.Send(new GetSentPagedQuery(UserId, query), ct);
            return Ok(result);
        }

        [HttpGet("sent/{emailId}")]
        public async Task<IActionResult> GetSentDetail(Guid emailId, CancellationToken ct)
        {
            var result = await _mediator.Send(new GetSentByIdQuery(UserId, emailId), ct);
            return Ok(result);
        }
    }
}
