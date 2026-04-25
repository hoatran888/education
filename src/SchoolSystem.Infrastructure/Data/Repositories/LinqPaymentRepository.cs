using SchoolSystem.Domain.Models.Entities;
using SchoolSystem.Domain.Models.Enums;
using SchoolSystem.Domain.Models.ValueObjects;
using SchoolSystem.Infrastructure.Data.Context;
using SchoolSystem.Infrastructure.Data.Interfaces;
using SchoolSystem.Infrastructure.Data.LinqEntities;

namespace SchoolSystem.Infrastructure.Data.Repositories;

public class LinqPaymentRepository
    : LinqRepositoryBase<PaymentEntity, Payment>, IPaymentRepository
{
    public LinqPaymentRepository(SchoolDataContext context, Guid schoolId)
        : base(context, schoolId) { }

    public async Task<Payment?> GetByIdAsync(Guid paymentId, CancellationToken ct = default) =>
        await FirstOrDefaultAsync(
            Table.Where(p => p.SchoolId == _schoolId && p.PaymentId == paymentId), ct);

    public async Task<IReadOnlyList<Payment>> GetByInvoiceAsync(
        Guid invoiceId, CancellationToken ct = default) =>
        await ToListAsync(
            Table.Where(p => p.SchoolId == _schoolId && p.InvoiceId == invoiceId)
                 .OrderBy(p => p.PaymentDate), ct);

    public async Task<IReadOnlyList<Payment>> GetByParentAsync(
        Guid parentUserId, CancellationToken ct = default) =>
        await ToListAsync(
            Table.Where(p => p.SchoolId == _schoolId && p.ParentUserId == parentUserId)
                 .OrderByDescending(p => p.PaymentDate), ct);

    public async Task AddAsync(Payment payment, CancellationToken ct = default) =>
        await Table.AddAsync(MapToEntity(payment), ct);

    public void Update(Payment payment) =>
        _context.Update(MapToEntity(payment));

    protected override Payment MapToDomain(PaymentEntity e) =>
        Payment.Reconstitute(
            e.PaymentId,
            e.SchoolId,
            e.InvoiceId,
            e.ParentUserId,
            new Money(e.Amount, e.Currency),
            e.PaymentDate,
            (PaymentMethod)e.Method,
            e.Reference,
            (PaymentStatus)e.Status,
            e.Notes,
            e.RecordedBy);

    protected override PaymentEntity MapToEntity(Payment p) => new()
    {
        PaymentId    = p.PaymentId,
        SchoolId     = p.SchoolId,
        InvoiceId    = p.InvoiceId,
        ParentUserId = p.ParentUserId,
        Amount       = p.Amount.Amount,
        Currency     = p.Amount.Currency,
        PaymentDate  = p.PaymentDate,
        Method       = (int)p.Method,
        Reference    = p.Reference,
        Status       = (int)p.Status,
        Notes        = p.Notes,
        RecordedBy   = p.RecordedBy
    };
}
