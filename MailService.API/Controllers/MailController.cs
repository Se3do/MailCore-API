using MailService.Application.Commands.Emails.SendEmail;
using MailService.Application.Commands.Emails.ForwardEmail;
using MailService.Application.Commands.Emails.ReplyEmail;
using MailService.Application.Queries.Email.GetSentPaged;
using MailService.Application.Queries.Email.GetSentById;
using MailService.Application.Common.Pagination;
using MailService.Application.DTOs.Emails;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MailService.API.Controllers
{
    [Authorize]
    [Route("api/mail")]
    [ApiController]
    public class MailController : ControllerBase
    {
        private readonly IMediator _mediator;

        public MailController(IMediator mediator)
        {
            _mediator = mediator;
        }
        private Guid UserId=>
            Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        [HttpPost("send")]
        public async Task<IActionResult> SendEmail([FromQuery] SendEmailRequest request, CancellationToken ct)
        {
            await _mediator.Send(new SendEmailCommand(UserId, request), ct);
            return NoContent();
        }

        [HttpPost("forward")]
        public async Task<IActionResult> ForwardEmail(Guid emailId, [FromQuery] ForwardEmailRequest request, CancellationToken ct)
        {
            await _mediator.Send(new ForwardEmailCommand(UserId, emailId, request), ct);
            return NoContent();
        }

        [HttpPost("reply")]
        public async Task<IActionResult> ReplyEmail(Guid emailId, [FromQuery] ReplyEmailRequest request, CancellationToken ct)
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
            if (result == null)
                return NotFound();
            return Ok(result);
        }
    }
}
