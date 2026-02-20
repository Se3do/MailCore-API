using MailService.API.Extensions;
using MailService.Application.Common.Pagination;
using MailService.Application.DTOs.Drafts;
using MailService.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MailService.API.Controllers
{
    [ApiController]
    [Route("api/drafts")]
    [Authorize]
    public sealed class DraftController : ControllerBase
    {
        private readonly IDraftService _draftService;

        public DraftController(IDraftService draftService)
        {
            _draftService = draftService;
        }

        [HttpGet]
        public async Task<IActionResult> GetPaged([FromQuery] CursorPaginationQuery query, CancellationToken ct)
        {
            var userId = User.GetUserId();
            var result = await _draftService.GetAllPagedAsync(userId, query, ct);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
        {
            var userId = User.GetUserId();
            var result = await _draftService.GetByIdAsync(userId, id, ct);
            return result is null ? NotFound() : Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateDraftRequest request, CancellationToken ct)
        {
            var userId = User.GetUserId();
            var id = await _draftService.CreateAsync(userId, request, ct);
            return CreatedAtAction(nameof(GetById), new { id }, null);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateDraftRequest request, CancellationToken ct)
        {
            var userId = User.GetUserId();
            var updated = await _draftService.UpdateAsync(userId, id, request, ct);
            return updated is false ? NotFound() : NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
        {
            var userId = User.GetUserId();
            return await _draftService.DeleteAsync(userId, id, ct) ? NoContent() : NotFound();
        }

        //TODO: Implement endpoint for sending draft as email
    }
}
