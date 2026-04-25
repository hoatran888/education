using MediatR;
using SchoolSystem.Application.Common.Interfaces;
using SchoolSystem.Application.Features.Payments.DTOs;
using SchoolSystem.Infrastructure.Data.Interfaces;

namespace SchoolSystem.Application.Features.Payments.Queries;

public record GetPaymentsByParentQuery(Guid ParentUserId) : IQuery<IReadOnlyList<PaymentDto>>;

public class GetPaymentsByParentQueryHandler
    : IRequestHandler<GetPaymentsByParentQuery, IReadOnlyList<PaymentDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetPaymentsByParentQueryHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<IReadOnlyList<PaymentDto>> Handle(
        GetPaymentsByParentQuery request, CancellationToken ct)
    {
        var payments = await _unitOfWork.Payments.GetByParentAsync(request.ParentUserId, ct);
        return payments.Select(GetPaymentsByInvoiceQueryHandler.ToDto).ToList();
    }
}
