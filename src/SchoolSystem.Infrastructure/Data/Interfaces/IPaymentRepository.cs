using SchoolSystem.Domain.Models.Entities;

namespace SchoolSystem.Infrastructure.Data.Interfaces;

public interface IPaymentRepository
{
    Task<Payment?>              GetByIdAsync(Guid paymentId, CancellationToken ct = default);
    Task<IReadOnlyList<Payment>> GetByInvoiceAsync(Guid invoiceId, CancellationToken ct = default);
    Task<IReadOnlyList<Payment>> GetByParentAsync(Guid parentUserId, CancellationToken ct = default);
    Task                        AddAsync(Payment payment, CancellationToken ct = default);
    void                        Update(Payment payment);
}
