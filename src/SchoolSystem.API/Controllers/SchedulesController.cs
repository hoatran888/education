using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolSystem.Application.Features.Schedules.Commands;
using SchoolSystem.Application.Features.Schedules.Queries;

namespace SchoolSystem.API.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class SchedulesController : ControllerBase
{
    private readonly ISender _sender;
    public SchedulesController(ISender sender) => _sender = sender;

    [HttpGet("by-course/{courseId:guid}")]
    public async Task<IActionResult> GetByCourse(Guid courseId, CancellationToken ct)
        => Ok(await _sender.Send(new GetSchedulesByCourseQuery(courseId), ct));

    [HttpGet("grid")]
    public async Task<IActionResult> GetGrid(
        [FromQuery] int month, [FromQuery] int year, CancellationToken ct)
        => Ok(await _sender.Send(new GetScheduleGridQuery(month, year), ct));

    [HttpPost]
    public async Task<IActionResult> Create(CreateScheduleCommand command, CancellationToken ct)
    {
        var id = await _sender.Send(command, ct);
        return Created($"api/schedules/{id}", new { id });
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await _sender.Send(new DeleteScheduleCommand(id), ct);
        return NoContent();
    }
}
