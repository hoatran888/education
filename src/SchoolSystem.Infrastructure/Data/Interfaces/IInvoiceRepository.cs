using SchoolSystem.Domain.Models.Entities;
using SchoolSystem.Domain.Models.Enums;

namespace SchoolSystem.Infrastructure.Data.Interfaces;

public interface IInvoiceRepository
{
    Task<Invoice?>              GetByIdAsync(Guid invoiceId, CancellationToken ct = default);
    Task<IReadOnlyList<Invoice>> GetByStudentAsync(Guid studentUserId, CancellationToken ct = default);
    Task<IReadOnlyList<Invoice>> GetByParentAsync(Guid parentUserId, CancellationToken ct = default);
    Task<IReadOnlyList<Invoice>> GetByStatusAsync(InvoiceStatus status, CancellationToken ct = default);
    Task<IReadOnlyList<Invoice>> GetOverdueAsync(CancellationToken ct = default);
    Task                        AddAsync(Invoice invoice, CancellationToken ct = default);
    void                        Update(Invoice invoice);
}
