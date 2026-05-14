using Asp.Versioning;
using MailCore.API.Extensions;
using MailCore.Application.Commands.Drafts.CreateDraft;
using MailCore.Application.Commands.Drafts.DeleteDraft;
using MailCore.Application.Commands.Drafts.SendDraft;
using MailCore.Application.Commands.Drafts.UpdateDraft;
using MailCore.Application.Common.Pagination;
using MailCore.Application.DTOs.Drafts;
using MailCore.Application.Queries.Drafts.GetDraftById;
using MailCore.Application.Queries.Drafts.GetDraftsPaged;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MailCore.API.Controllers
{
    /// <summary>Handles draft email operations including creating, updating, and sending drafts.</summary>
    [ApiController]
    [Route("api/v{version:apiVersion}/drafts")]
    [ApiVersion("1.0")]
    [Authorize]
    public sealed class DraftController : ControllerBase
    {
        private readonly IMediator _mediator;
        public DraftController(IMediator mediator) => _mediator = mediator;

        private Guid UserId => User.GetUserId();

        /// <summary>Gets a paginated list of drafts for the current user.</summary>
        /// <param name="query">Pagination cursor parameters.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>A cursor-paginated result of <see cref="DraftDto"/>.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(CursorPagedResult<DraftDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetPaged([FromQuery] CursorPaginationQuery query, CancellationToken ct)
        {
            var result = await _mediator.Send(new GetDraftsPagedQuery(UserId, query), ct);
            return Ok(result);
        }

        /// <summary>Gets a draft by its ID.</summary>
        /// <param name="id">The draft ID.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>A <see cref="DraftDto"/> with the draft details.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(DraftDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
        {
            var result = await _mediator.Send(new GetDraftByIdQuery(UserId, id), ct);
            return result is null ? NotFound() : Ok(result);
        }

        /// <summary>Creates a new draft.</summary>
        /// <param name="request">The draft creation details.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>The ID of the newly created draft.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Create([FromBody] CreateDraftRequest request, CancellationToken ct)
        {
            var id = await _mediator.Send(new CreateDraftCommand(UserId, request), ct);
            return CreatedAtAction(nameof(GetById), new { id }, null);
        }

        /// <summary>Updates an existing draft.</summary>
        /// <param name="id">The draft ID.</param>
        /// <param name="request">The updated draft details.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>No content if successful; 404 if the draft does not exist.</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateDraftRequest request, CancellationToken ct)
        {
            var updated = await _mediator.Send(new UpdateDraftCommand(UserId, id, request), ct);
            return updated is false ? NotFound() : NoContent();
        }

        /// <summary>Deletes a draft.</summary>
        /// <param name="id">The draft ID.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>No content if successful; 404 if the draft does not exist.</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
        {
            return await _mediator.Send(new DeleteDraftCommand(UserId, id), ct) ? NoContent() : NotFound();
        }

        /// <summary>Sends a draft, converting it to a sent email.</summary>
        /// <param name="id">The draft ID.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>No content.</returns>
        [HttpPost("{id}/send")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Send(Guid id, CancellationToken ct)
        {
            await _mediator.Send(new SendDraftCommand(UserId, id), ct);
            return NoContent();
        }
    }
}
