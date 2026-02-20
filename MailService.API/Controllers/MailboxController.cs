    using MailService.API.Extensions;
    using MailService.Application.Common.Pagination;
    using MailService.Application.Services.Interfaces;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    namespace MailService.API.Controllers
    {
        [Route("api/[controller]")]
        [ApiController]
        [Authorize]
        public class MailboxController : ControllerBase
        {
            private readonly IMailboxService _mailboxService;

            public MailboxController(IMailboxService mailboxService)
            {
                _mailboxService = mailboxService;
            }

            // ===================== READ =====================

            [HttpGet("inbox")]
            public async Task<IActionResult> GetInbox([FromQuery] CursorPaginationQuery query, CancellationToken ct)
            {
                var userId = User.GetUserId();
                var result = await _mailboxService.GetInboxPagedAsync(userId, query, ct);
                return Ok(result);
            }

            [HttpGet("unread")]
            public async Task<IActionResult> GetUnread([FromQuery] CursorPaginationQuery query, CancellationToken ct)
            {
                var userId = User.GetUserId();
                var result = await _mailboxService.GetUnreadPagedAsync(userId, query, ct);
                return Ok(result);
            }

            [HttpGet("starred")]
            public async Task<IActionResult> GetStarred([FromQuery] CursorPaginationQuery query, CancellationToken ct)
            {
                var userId = User.GetUserId();
                var result = await _mailboxService.GetStarredPagedAsync(userId, query, ct);
                return Ok(result);
            }

            [HttpGet("spam")]
            public async Task<IActionResult> GetSpam([FromQuery] CursorPaginationQuery query, CancellationToken ct)
            {
                var userId = User.GetUserId();
                var result = await _mailboxService.GetSpamPagedAsync(userId, query, ct);
                return Ok(result);
            }

            [HttpGet("trash")]
            public async Task<IActionResult> GetTrash([FromQuery] CursorPaginationQuery query, CancellationToken ct)
            {
                var userId = User.GetUserId();
                var result = await _mailboxService.GetTrashPagedAsync(userId, query, ct);
                return Ok(result);
            }

            [HttpGet("thread/{threadId}")]
            public async Task<IActionResult> GetByThread(Guid threadId, [FromQuery] CursorPaginationQuery query, CancellationToken ct)
            {
                var userId = User.GetUserId();
                var result = await _mailboxService.GetByThreadPagedAsync(userId, threadId, query, ct);
                return Ok(result);
            }

            [HttpGet("{mailRecipientId}")]
            public async Task<IActionResult> GetById(Guid mailId, CancellationToken ct)
            {
                var userId = User.GetUserId();
                var result = await _mailboxService.GetMailByIdAsync(userId, mailId, ct);
                return result is null ? NotFound() : Ok(result);
            }

            // ===================== WRITE =====================

            [HttpPost("{mailRecipientId}/read")]
            public async Task<IActionResult> MarkRead(Guid mailRecipientId, CancellationToken ct)
            {
                var userId = User.GetUserId();
                return await _mailboxService.MarkReadAsync(userId, mailRecipientId, ct)
                    ? NoContent()
                    : NotFound();
            }

            [HttpPost("{mailRecipientId}/unread")]
            public async Task<IActionResult> MarkUnread(Guid mailRecipientId, CancellationToken ct)
            {
                var userId = User.GetUserId();
                return await _mailboxService.MarkUnreadAsync(userId, mailRecipientId, ct)
                    ? NoContent()
                    : NotFound();
            }

            [HttpPost("{mailRecipientId}/spam")]
            public async Task<IActionResult> MarkSpam(Guid mailRecipientId, CancellationToken ct)
            {
                var userId = User.GetUserId();
                return await _mailboxService.MarkSpamAsync(userId, mailRecipientId, ct)
                    ? NoContent()
                    : NotFound();
            }

            [HttpPost("{mailRecipientId}/unspam")]
            public async Task<IActionResult> Unspam(Guid mailRecipientId, CancellationToken ct)
            {
                var userId = User.GetUserId();
                return await _mailboxService.UnspamAsync(userId, mailRecipientId, ct)
                    ? NoContent()
                    : NotFound();
            }

            [HttpPost("{mailRecipientId}/star")]
            public async Task<IActionResult> Star(Guid mailRecipientId, CancellationToken ct)
            {
                var userId = User.GetUserId();
                return await _mailboxService.StarAsync(userId, mailRecipientId, ct)
                    ? NoContent()
                    : NotFound();
            }

            [HttpPost("{mailRecipientId}/unstar")]
            public async Task<IActionResult> Unstar(Guid mailRecipientId, CancellationToken ct)
            {
                var userId = User.GetUserId();
                return await _mailboxService.UnstarAsync(userId, mailRecipientId, ct)
                    ? NoContent()
                    : NotFound();
            }

            [HttpDelete("{mailRecipientId}")]
            public async Task<IActionResult> Delete(Guid mailRecipientId, CancellationToken ct)
            {
                var userId = User.GetUserId();
                return await _mailboxService.DeleteAsync(userId, mailRecipientId, ct)
                    ? NoContent()
                    : NotFound();
            }

            [HttpPost("{mailRecipientId}/restore")]
            public async Task<IActionResult> Restore(Guid mailRecipientId, CancellationToken ct)
            {
                var userId = User.GetUserId();
                return await _mailboxService.RestoreAsync(userId, mailRecipientId, ct)
                    ? NoContent()
                    : NotFound();
            }
        }
    }
