using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolSystem.Application.Features.Users.Commands;
using SchoolSystem.Application.Features.Users.Queries;
using SchoolSystem.Domain.Models.Enums;

namespace SchoolSystem.API.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly ISender _sender;
    public UsersController(ISender sender) => _sender = sender;

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
        => Ok(await _sender.Send(new GetUserByIdQuery(id), ct));

    [HttpGet("by-role/{role}")]
    public async Task<IActionResult> GetByRole(UserRole role, CancellationToken ct)
        => Ok(await _sender.Send(new GetUsersByRoleQuery(role), ct));

    [HttpPost]
    public async Task<IActionResult> Create(CreateUserCommand command, CancellationToken ct)
    {
        var id = await _sender.Send(command, ct);
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id, UpdateUserRequest request, CancellationToken ct)
    {
        await _sender.Send(new UpdateUserCommand(
            id, request.FirstName, request.LastName, request.Phone,
            request.Street, request.City, request.State, request.ZipCode, request.Country), ct);
        return NoContent();
    }

    [HttpPost("{id:guid}/roles/assign")]
    public async Task<IActionResult> AssignRole(
        Guid id, AssignRoleRequest request, CancellationToken ct)
    {
        await _sender.Send(new AssignRoleCommand(id, request.Role), ct);
        return NoContent();
    }

    [HttpPost("{id:guid}/roles/remove")]
    public async Task<IActionResult> RemoveRole(
        Guid id, RemoveRoleRequest request, CancellationToken ct)
    {
        await _sender.Send(new RemoveRoleCommand(id, request.Role), ct);
        return NoContent();
    }

    [HttpPost("{id:guid}/deactivate")]
    public async Task<IActionResult> Deactivate(Guid id, CancellationToken ct)
    {
        await _sender.Send(new DeactivateUserCommand(id), ct);
        return NoContent();
    }
}

public record UpdateUserRequest(
    string FirstName, string LastName, string Phone,
    string Street, string City, string State, string ZipCode, string Country);

public record AssignRoleRequest(UserRole Role);
public record RemoveRoleRequest(UserRole Role);
