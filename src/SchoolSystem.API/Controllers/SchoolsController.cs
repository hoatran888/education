using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolSystem.Application.Features.Schools.Commands;
using SchoolSystem.Application.Features.Schools.Queries;

namespace SchoolSystem.API.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class SchoolsController : ControllerBase
{
    private readonly ISender _sender;
    public SchoolsController(ISender sender) => _sender = sender;

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
        => Ok(await _sender.Send(new GetAllSchoolsQuery(), ct));

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
        => Ok(await _sender.Send(new GetSchoolByIdQuery(id), ct));

    [HttpPost]
    public async Task<IActionResult> Create(CreateSchoolCommand command, CancellationToken ct)
    {
        var id = await _sender.Send(command, ct);
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id, UpdateSchoolRequest request, CancellationToken ct)
    {
        await _sender.Send(new UpdateSchoolCommand(
            id, request.Name, request.Street, request.City,
            request.State, request.ZipCode, request.Country,
            request.Email, request.Phone), ct);
        return NoContent();
    }
}

public record UpdateSchoolRequest(
    string Name, string Street, string City, string State,
    string ZipCode, string Country, string Email, string Phone);
