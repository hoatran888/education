using MediatR;
using SchoolSystem.Application.Common.Interfaces;
using SchoolSystem.Application.Features.Invoices.DTOs;
using SchoolSystem.Infrastructure.Data.Interfaces;

namespace SchoolSystem.Application.Features.Invoices.Queries;

public record GetInvoicesByStudentQuery(Guid StudentUserId) : IQuery<IReadOnlyList<InvoiceDto>>;

public class GetInvoicesByStudentQueryHandler
    : IRequestHandler<GetInvoicesByStudentQuery, IReadOnlyList<InvoiceDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetInvoicesByStudentQueryHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<IReadOnlyList<InvoiceDto>> Handle(
        GetInvoicesByStudentQuery request, CancellationToken ct)
    {
        var invoices = await _unitOfWork.Invoices.GetByStudentAsync(request.StudentUserId, ct);
        return invoices.Select(GetInvoiceByIdQueryHandler.ToDto).ToList();
    }
}
