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
    [Route("api/v{version:apiVersion}/labels")]
    [ApiController]
    [ApiVersion("1.0")]
    [Authorize]
    public class LabelController : ControllerBase
    {
        private readonly IMediator _mediator;
        public LabelController(IMediator mediator) => _mediator = mediator;

        private Guid UserId => User.GetUserId();

        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<LabelDto>>> GetAll(CancellationToken ct)
        {
            var labels = await _mediator.Send(new GetAllLabelsQuery(UserId), ct);
            return Ok(labels);
        }

        [HttpPost]
        public async Task<ActionResult<Guid>> Create([FromBody] CreateLabelRequest request, CancellationToken ct)
        {
            var labelId = await _mediator.Send(new CreateLabelCommand(UserId, request), ct);
            return CreatedAtAction(nameof(GetAll), new { id = labelId }, labelId);
        }

        [HttpPut("{labelId}")]
        public async Task<IActionResult> Update(Guid labelId, [FromBody] UpdateLabelRequest request, CancellationToken ct)
        {
            var updated = await _mediator.Send(new UpdateLabelCommand(UserId, labelId, request), ct);
            return updated ? NoContent() : NotFound();
        }

        [HttpDelete("{labelId}")]
        public async Task<IActionResult> Delete(Guid labelId, CancellationToken ct)
        {
            var deleted = await _mediator.Send(new DeleteLabelCommand(UserId, labelId), ct);
            return deleted ? NoContent() : NotFound();
        }

        [HttpPost("{labelId}/assign/{mailId}")]
        public async Task<IActionResult> AssignLabel(Guid labelId, Guid mailId, CancellationToken ct)
        {
            var assigned = await _mediator.Send(new AssignLabelCommand(UserId, mailId, labelId), ct);
            return assigned ? NoContent() : BadRequest();
        }

        [HttpDelete("{labelId}/unassign/{mailId}")]
        public async Task<IActionResult> UnassignLabel(Guid labelId, Guid mailId, CancellationToken ct)
        {
            var unassigned = await _mediator.Send(new UnassignLabelCommand(UserId, mailId, labelId), ct);
            return unassigned ? NoContent() : BadRequest();
        }
    }
}
