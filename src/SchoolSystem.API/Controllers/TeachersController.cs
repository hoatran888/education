using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolSystem.Application.Features.Teachers.Commands;
using SchoolSystem.Application.Features.Teachers.Queries;

namespace SchoolSystem.API.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class TeachersController : ControllerBase
{
    private readonly ISender _sender;
    public TeachersController(ISender sender) => _sender = sender;

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
        => Ok(await _sender.Send(new GetAllTeachersQuery(), ct));

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
        => Ok(await _sender.Send(new GetTeacherByIdQuery(id), ct));

    [HttpPost]
    public async Task<IActionResult> Create(CreateTeacherProfileCommand command, CancellationToken ct)
    {
        var id = await _sender.Send(command, ct);
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id, UpdateTeacherRequest request, CancellationToken ct)
    {
        await _sender.Send(
            new UpdateTeacherProfileCommand(id, request.Degree, request.Specialization), ct);
        return NoContent();
    }
}

public record UpdateTeacherRequest(string Degree, string Specialization);
