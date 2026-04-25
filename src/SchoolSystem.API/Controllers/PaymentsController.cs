using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolSystem.Application.Features.Payments.Commands;
using SchoolSystem.Application.Features.Payments.Queries;

namespace SchoolSystem.API.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class PaymentsController : ControllerBase
{
    private readonly ISender _sender;
    public PaymentsController(ISender sender) => _sender = sender;

    [HttpGet("by-invoice/{invoiceId:guid}")]
    public async Task<IActionResult> GetByInvoice(Guid invoiceId, CancellationToken ct)
        => Ok(await _sender.Send(new GetPaymentsByInvoiceQuery(invoiceId), ct));

    [HttpGet("by-parent/{parentUserId:guid}")]
    public async Task<IActionResult> GetByParent(Guid parentUserId, CancellationToken ct)
        => Ok(await _sender.Send(new GetPaymentsByParentQuery(parentUserId), ct));

    [HttpPost]
    public async Task<IActionResult> Record(RecordPaymentCommand command, CancellationToken ct)
    {
        var id = await _sender.Send(command, ct);
        return Created($"api/payments/{id}", new { id });
    }

    [HttpPost("{id:guid}/confirm")]
    public async Task<IActionResult> Confirm(Guid id, CancellationToken ct)
    {
        await _sender.Send(new ConfirmPaymentCommand(id), ct);
        return NoContent();
    }
}
