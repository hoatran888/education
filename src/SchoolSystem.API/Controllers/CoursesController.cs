using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolSystem.Application.Features.Courses.Commands;
using SchoolSystem.Application.Features.Courses.Queries;

namespace SchoolSystem.API.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class CoursesController : ControllerBase
{
    private readonly ISender _sender;
    public CoursesController(ISender sender) => _sender = sender;

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
        => Ok(await _sender.Send(new GetCourseByIdQuery(id), ct));

    [HttpGet("by-year/{academicYearId:guid}")]
    public async Task<IActionResult> GetByAcademicYear(Guid academicYearId, CancellationToken ct)
        => Ok(await _sender.Send(new GetCoursesByAcademicYearQuery(academicYearId), ct));

    [HttpGet("open")]
    public async Task<IActionResult> GetOpen(CancellationToken ct)
        => Ok(await _sender.Send(new GetOpenCoursesQuery(), ct));

    [HttpPost]
    public async Task<IActionResult> Create(CreateCourseCommand command, CancellationToken ct)
    {
        var id = await _sender.Send(command, ct);
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id, UpdateCourseRequest request, CancellationToken ct)
    {
        await _sender.Send(
            new UpdateCourseCommand(id, request.Name, request.Description, request.MaxStudents), ct);
        return NoContent();
    }

    [HttpPost("{id:guid}/teacher")]
    public async Task<IActionResult> AssignTeacher(
        Guid id, AssignTeacherRequest request, CancellationToken ct)
    {
        await _sender.Send(new AssignTeacherToCourseCommand(id, request.TeacherUserId), ct);
        return NoContent();
    }

    [HttpPost("{id:guid}/open")]
    public async Task<IActionResult> Open(Guid id, CancellationToken ct)
    {
        await _sender.Send(new OpenCourseCommand(id), ct);
        return NoContent();
    }

    [HttpPost("{id:guid}/close")]
    public async Task<IActionResult> Close(Guid id, CancellationToken ct)
    {
        await _sender.Send(new CloseCourseCommand(id), ct);
        return NoContent();
    }
}

public record UpdateCourseRequest(string Name, string? Description, int MaxStudents);
public record AssignTeacherRequest(Guid TeacherUserId);
