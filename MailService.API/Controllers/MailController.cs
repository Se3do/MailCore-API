using MailService.API.Extensions;
using MailService.Application.Common.Pagination;
using MailService.Application.DTOs.Emails;
using MailService.Application.Services;
using MailService.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MailService.API.Controllers
{
    [Authorize]
    [Route("api/mail")]
    [ApiController]
    public class MailController : ControllerBase
    {
        private readonly IEmailService _emailService;

        public MailController(IEmailService emailService)
        {
            _emailService = emailService;
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendEmail([FromQuery] SendEmailRequest request, CancellationToken ct)
        {
            var userId = User.GetUserId();
            await _emailService.SendAsync(userId, request, ct);
            return NoContent();
        }

        [HttpPost("forward")]
        public async Task<IActionResult> ForwardEmail(Guid emailId, [FromQuery] ForwardEmailRequest request, CancellationToken ct)
        {
            var userId = User.GetUserId();
            await _emailService.ForwardAsync(userId, emailId, request, ct);
            return NoContent();
        }

        [HttpPost("reply")]
        public async Task<IActionResult> ReplyEmail(Guid emailId, [FromQuery] ReplyEmailRequest request, CancellationToken ct)
        {
            var userId = User.GetUserId();
            await _emailService.ReplyAsync(userId, emailId, request, ct);
            return NoContent();

        }

        [HttpGet("sent")]
        public async Task<IActionResult> GetSent([FromQuery] CursorPaginationQuery query, CancellationToken ct)
        {
            var userId = User.GetUserId();
            var result = await _emailService.GetSentPagedAsync(userId, query, ct);
            return Ok(result);
        }

        [HttpGet("sent/{emailId}")]
        public async Task<IActionResult> GetSentDetail(Guid emailId, CancellationToken ct)
        {
            var userId = User.GetUserId();
            var result = await _emailService.GetSentByIdAsync(userId, emailId, ct);
            if (result == null)
                return NotFound();
            return Ok(result);
        }
    }
}
