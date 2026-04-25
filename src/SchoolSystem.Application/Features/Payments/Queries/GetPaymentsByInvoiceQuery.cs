using MediatR;
using SchoolSystem.Application.Common.Interfaces;
using SchoolSystem.Application.Features.Payments.DTOs;
using SchoolSystem.Domain.Models.Entities;
using SchoolSystem.Infrastructure.Data.Interfaces;

namespace SchoolSystem.Application.Features.Payments.Queries;

public record GetPaymentsByInvoiceQuery(Guid InvoiceId) : IQuery<IReadOnlyList<PaymentDto>>;

public class GetPaymentsByInvoiceQueryHandler
    : IRequestHandler<GetPaymentsByInvoiceQuery, IReadOnlyList<PaymentDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetPaymentsByInvoiceQueryHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<IReadOnlyList<PaymentDto>> Handle(
        GetPaymentsByInvoiceQuery request, CancellationToken ct)
    {
        var payments = await _unitOfWork.Payments.GetByInvoiceAsync(request.InvoiceId, ct);
        return payments.Select(ToDto).ToList();
    }

    internal static PaymentDto ToDto(Payment p) => new(
        p.PaymentId, p.SchoolId, p.InvoiceId, p.ParentUserId,
        p.Amount.Amount, p.Amount.Currency,
        p.PaymentDate, p.Method.ToString(), p.Reference,
        p.Status.ToString(), p.Notes, p.RecordedBy);
}
