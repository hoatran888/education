using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolSystem.Application.Features.Grades.Commands;
using SchoolSystem.Application.Features.Grades.Queries;

namespace SchoolSystem.API.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class GradesController : ControllerBase
{
    private readonly ISender _sender;
    public GradesController(ISender sender) => _sender = sender;

    [HttpGet("by-enrollment/{enrollmentId:guid}")]
    public async Task<IActionResult> GetByEnrollment(Guid enrollmentId, CancellationToken ct)
        => Ok(await _sender.Send(new GetGradesByEnrollmentQuery(enrollmentId), ct));

    [HttpGet("report/{studentUserId:guid}")]
    public async Task<IActionResult> GetStudentReport(Guid studentUserId, CancellationToken ct)
        => Ok(await _sender.Send(new GetStudentReportQuery(studentUserId), ct));

    [HttpPost]
    public async Task<IActionResult> Record(RecordGradeCommand command, CancellationToken ct)
    {
        var id = await _sender.Send(command, ct);
        return Created($"api/grades/{id}", new { id });
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id, UpdateGradeRequest request, CancellationToken ct)
    {
        await _sender.Send(new UpdateGradeCommand(id, request.Score, request.Comment), ct);
        return NoContent();
    }

    [HttpPost("{id:guid}/publish")]
    public async Task<IActionResult> Publish(Guid id, CancellationToken ct)
    {
        await _sender.Send(new PublishGradeCommand(id), ct);
        return NoContent();
    }

    [HttpPost("publish-all")]
    public async Task<IActionResult> PublishAll(PublishAllGradesCommand command, CancellationToken ct)
    {
        await _sender.Send(command, ct);
        return NoContent();
    }
}

public record UpdateGradeRequest(decimal Score, string? Comment);
