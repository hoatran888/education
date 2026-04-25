using MediatR;
using SchoolSystem.Application.Common.Exceptions;
using SchoolSystem.Application.Common.Interfaces;
using SchoolSystem.Application.Features.Invoices.DTOs;
using SchoolSystem.Domain.Models.Entities;
using SchoolSystem.Infrastructure.Data.Interfaces;

namespace SchoolSystem.Application.Features.Invoices.Queries;

public record GetInvoiceByIdQuery(Guid InvoiceId) : IQuery<InvoiceDto>;

public class GetInvoiceByIdQueryHandler : IRequestHandler<GetInvoiceByIdQuery, InvoiceDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetInvoiceByIdQueryHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<InvoiceDto> Handle(GetInvoiceByIdQuery request, CancellationToken ct)
    {
        var invoice = await _unitOfWork.Invoices.GetByIdAsync(request.InvoiceId, ct)
            ?? throw new NotFoundException(nameof(Invoice), request.InvoiceId);
        return ToDto(invoice);
    }

    internal static InvoiceDto ToDto(Invoice inv) => new(
        inv.InvoiceId, inv.SchoolId, inv.StudentUserId, inv.ParentUserId, inv.AcademicYearId,
        inv.TotalAmount.Amount, inv.PaidAmount.Amount, inv.OutstandingBalance.Amount,
        inv.TotalAmount.Currency,
        inv.Status.ToString(), inv.DueDate, inv.IssuedDate, inv.PaidDate, inv.Notes,
        inv.Items.Select(i => new InvoiceItemDto(
            i.InvoiceItemId, i.CourseId, i.Description, i.Amount.Amount, i.Amount.Currency))
            .ToList());
}
