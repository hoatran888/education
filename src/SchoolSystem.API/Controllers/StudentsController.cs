using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolSystem.Application.Features.Students.Commands;
using SchoolSystem.Application.Features.Students.Queries;

namespace SchoolSystem.API.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class StudentsController : ControllerBase
{
    private readonly ISender _sender;
    public StudentsController(ISender sender) => _sender = sender;

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
        => Ok(await _sender.Send(new GetStudentByIdQuery(id), ct));

    [HttpGet("by-grade/{gradeLevel:int}")]
    public async Task<IActionResult> GetByGradeLevel(int gradeLevel, CancellationToken ct)
        => Ok(await _sender.Send(new GetStudentsByGradeLevelQuery(gradeLevel), ct));

    [HttpPost]
    public async Task<IActionResult> Create(CreateStudentProfileCommand command, CancellationToken ct)
    {
        var id = await _sender.Send(command, ct);
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [HttpPut("{id:guid}/grade-level")]
    public async Task<IActionResult> UpdateGradeLevel(
        Guid id, UpdateGradeLevelRequest request, CancellationToken ct)
    {
        await _sender.Send(new UpdateStudentGradeLevelCommand(id, request.GradeLevel), ct);
        return NoContent();
    }
}

public record UpdateGradeLevelRequest(int GradeLevel);
