using Asp.Versioning;
using MailCore.API.Extensions;
using MailCore.Application.Commands.Labels.AssignLabel;
using MailCore.Application.Commands.Labels.CreateLabel;
using MailCore.Application.Commands.Labels.DeleteLabel;
using MailCore.Application.Commands.Labels.UnassignLabel;
using MailCore.Application.Commands.Labels.UpdateLabel;
using MailCore.Application.DTOs.Labels;
using MailCore.Application.Queries.Labels.GetAllLabels;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MailCore.API.Controllers
{
    /// <summary>Handles label management operations for organizing emails.</summary>
    [Route("api/v{version:apiVersion}/labels")]
    [ApiController]
    [ApiVersion("1.0")]
    [Authorize]
    public class LabelController : ControllerBase
    {
        private readonly IMediator _mediator;
        public LabelController(IMediator mediator) => _mediator = mediator;

        private Guid UserId => User.GetUserId();

        /// <summary>Gets all labels for the current user.</summary>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>A list of <see cref="LabelDto"/>.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IReadOnlyList<LabelDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<IReadOnlyList<LabelDto>>> GetAll(CancellationToken ct)
        {
            var labels = await _mediator.Send(new GetAllLabelsQuery(UserId), ct);
            return Ok(labels);
        }

        /// <summary>Creates a new label.</summary>
        /// <param name="request">The label creation details.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>The ID of the newly created label.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<Guid>> Create([FromBody] CreateLabelRequest request, CancellationToken ct)
        {
            var labelId = await _mediator.Send(new CreateLabelCommand(UserId, request), ct);
            return CreatedAtAction(nameof(GetAll), new { id = labelId }, labelId);
        }

        /// <summary>Updates an existing label.</summary>
        /// <param name="labelId">The ID of the label to update.</param>
        /// <param name="request">The updated label details.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>No content if successful; 404 if the label does not exist.</returns>
        [HttpPut("{labelId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Update(Guid labelId, [FromBody] UpdateLabelRequest request, CancellationToken ct)
        {
            var updated = await _mediator.Send(new UpdateLabelCommand(UserId, labelId, request), ct);
            return updated ? NoContent() : NotFound();
        }

        /// <summary>Deletes a label.</summary>
        /// <param name="labelId">The ID of the label to delete.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>No content if successful; 404 if the label does not exist.</returns>
        [HttpDelete("{labelId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Delete(Guid labelId, CancellationToken ct)
        {
            var deleted = await _mediator.Send(new DeleteLabelCommand(UserId, labelId), ct);
            return deleted ? NoContent() : NotFound();
        }

        /// <summary>Assigns a label to an email.</summary>
        /// <param name="labelId">The label ID.</param>
        /// <param name="mailId">The mail (recipient) ID.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>No content if successful; 400 if the assignment fails.</returns>
        [HttpPost("{labelId}/assign/{mailId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> AssignLabel(Guid labelId, Guid mailId, CancellationToken ct)
        {
            var assigned = await _mediator.Send(new AssignLabelCommand(UserId, mailId, labelId), ct);
            return assigned ? NoContent() : BadRequest();
        }

        /// <summary>Removes a label from an email.</summary>
        /// <param name="labelId">The label ID.</param>
        /// <param name="mailId">The mail (recipient) ID.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>No content if successful; 400 if the unassignment fails.</returns>
        [HttpDelete("{labelId}/unassign/{mailId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> UnassignLabel(Guid labelId, Guid mailId, CancellationToken ct)
        {
            var unassigned = await _mediator.Send(new UnassignLabelCommand(UserId, mailId, labelId), ct);
            return unassigned ? NoContent() : BadRequest();
        }
    }
}
