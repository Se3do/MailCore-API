using MailService.API.Extensions;
using MailService.Application.Commands.Drafts.CreateDraft;
using MailService.Application.Commands.Drafts.DeleteDraft;
using MailService.Application.Commands.Drafts.UpdateDraft;
using MailService.Application.Common.Pagination;
using MailService.Application.DTOs.Drafts;
using MailService.Application.Queries.Drafts.GetDraftById;
using MailService.Application.Queries.Drafts.GetDraftsPaged;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MailService.API.Controllers
{
    [ApiController]
    [Route("api/drafts")]
    [Authorize]
    public sealed class DraftController : ControllerBase
    {
        private readonly IMediator _mediator;
        public DraftController(IMediator mediator)
        {
            _mediator = mediator;
        }

        private Guid UserId =>
            Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        [HttpGet]
        public async Task<IActionResult> GetPaged([FromQuery] CursorPaginationQuery query, CancellationToken ct)
        {
            var result = await _mediator.Send(new GetDraftsPagedQuery(UserId, query), ct);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
        {
            var result = await _mediator.Send(new GetDraftByIdQuery(UserId, id), ct);
            return result is null ? NotFound() : Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateDraftRequest request, CancellationToken ct)
        {
            var id = await _mediator.Send(new CreateDraftCommand (UserId, request), ct);
            return CreatedAtAction(nameof(GetById), new { id }, null);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateDraftRequest request, CancellationToken ct)
        {
            var updated = await _mediator.Send(new UpdateDraftCommand(UserId, id, request), ct);
            return updated is false ? NotFound() : NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
        {
            return await _mediator.Send(new DeleteDraftCommand(UserId, id)) ? NoContent() : NotFound();
        }

        //TODO: Implement endpoint for sending draft as email
    }
}
