using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolSystem.Application.Features.Invoices.Commands;
using SchoolSystem.Application.Features.Invoices.Queries;

namespace SchoolSystem.API.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class InvoicesController : ControllerBase
{
    private readonly ISender _sender;
    public InvoicesController(ISender sender) => _sender = sender;

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
        => Ok(await _sender.Send(new GetInvoiceByIdQuery(id), ct));

    [HttpGet("by-student/{studentUserId:guid}")]
    public async Task<IActionResult> GetByStudent(Guid studentUserId, CancellationToken ct)
        => Ok(await _sender.Send(new GetInvoicesByStudentQuery(studentUserId), ct));

    [HttpPost]
    public async Task<IActionResult> Create(CreateInvoiceCommand command, CancellationToken ct)
    {
        var id = await _sender.Send(command, ct);
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [HttpPost("{id:guid}/send")]
    public async Task<IActionResult> Send(Guid id, CancellationToken ct)
    {
        await _sender.Send(new SendInvoiceCommand(id), ct);
        return NoContent();
    }

    [HttpPost("{id:guid}/waive")]
    public async Task<IActionResult> Waive(
        Guid id, WaiveInvoiceRequest request, CancellationToken ct)
    {
        await _sender.Send(new WaiveInvoiceCommand(id, request.Reason), ct);
        return NoContent();
    }
}

public record WaiveInvoiceRequest(string Reason);
