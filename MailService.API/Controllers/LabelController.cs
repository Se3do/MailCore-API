using MailService.Application.DTOs.Labels;
using MailService.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MailService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class LabelController : ControllerBase
    {
        private readonly ILabelService _labelService;

        public LabelController(ILabelService labelService)
        {
            _labelService = labelService;
        }

        private Guid UserId =>
            Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<LabelDto>>> GetAll(
            CancellationToken ct)
        {
            var labels = await _labelService.GetAllAsync(UserId, ct);
            return Ok(labels);
        }

        [HttpPost]
        public async Task<ActionResult<Guid>> Create([FromBody] CreateLabelRequest request, CancellationToken ct)
        {
            var labelId = await _labelService.CreateAsync(UserId, request, ct);
            return CreatedAtAction(nameof(GetAll), new { id = labelId }, labelId);
        }

        [HttpPut("{labelId}")]
        public async Task<IActionResult> Update(Guid labelId, [FromBody] UpdateLabelRequest request, CancellationToken ct)
        {
            var updated = await _labelService.UpdateAsync(UserId, labelId, request, ct);
            return updated ? NoContent() : NotFound();
        }

        [HttpDelete("{labelId}")]
        public async Task<IActionResult> Delete(Guid labelId, CancellationToken ct)
        {
            var deleted = await _labelService.DeleteAsync(UserId, labelId, ct);
            return deleted ? NoContent() : NotFound();
        }

        [HttpPost("{labelId}/assign/{mailId}")]
        public async Task<IActionResult> AssignLabel(Guid labelId, Guid mailId, CancellationToken ct)
        {
            var assigned = await _labelService.AssignLabelAsync(UserId, mailId, labelId, ct);
            return assigned ? NoContent() : BadRequest();
        }

        [HttpDelete("{labelId}/unassign/{mailId}")]
        public async Task<IActionResult> UnassignLabel(Guid labelId, Guid mailId, CancellationToken ct)
        {
            var unassigned = await _labelService.UnassignLabelAsync(UserId, mailId, labelId, ct);
            return unassigned ? NoContent() : BadRequest();
        }
    }
}
