using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolSystem.Application.Features.Enrollments.Commands;
using SchoolSystem.Application.Features.Enrollments.Queries;

namespace SchoolSystem.API.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class EnrollmentsController : ControllerBase
{
    private readonly ISender _sender;
    public EnrollmentsController(ISender sender) => _sender = sender;

    [HttpGet("by-student/{studentUserId:guid}")]
    public async Task<IActionResult> GetByStudent(Guid studentUserId, CancellationToken ct)
        => Ok(await _sender.Send(new GetEnrollmentsByStudentQuery(studentUserId), ct));

    [HttpGet("by-course/{courseId:guid}")]
    public async Task<IActionResult> GetByCourse(Guid courseId, CancellationToken ct)
        => Ok(await _sender.Send(new GetEnrollmentsByCourseQuery(courseId), ct));

    [HttpPost]
    public async Task<IActionResult> Enroll(EnrollStudentCommand command, CancellationToken ct)
    {
        var id = await _sender.Send(command, ct);
        return Created($"api/enrollments/{id}", new { id });
    }

    [HttpPost("{id:guid}/drop")]
    public async Task<IActionResult> Drop(
        Guid id, DropEnrollmentRequest request, CancellationToken ct)
    {
        await _sender.Send(new DropEnrollmentCommand(id, request.Reason), ct);
        return NoContent();
    }
}

public record DropEnrollmentRequest(string Reason);
