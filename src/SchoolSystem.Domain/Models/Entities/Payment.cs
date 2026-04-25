using SchoolSystem.Domain.Models.Enums;
using SchoolSystem.Domain.Models.ValueObjects;

namespace SchoolSystem.Domain.Models.Entities;

public class Payment
{
    public Guid          PaymentId    { get; private set; }
    public Guid          SchoolId     { get; private set; }
    public Guid          InvoiceId    { get; private set; }
    public Guid          ParentUserId { get; private set; }
    public Money         Amount       { get; private set; }
    public DateTime      PaymentDate  { get; private set; }
    public PaymentMethod Method       { get; private set; }
    public string        Reference    { get; private set; }
    public PaymentStatus Status       { get; private set; }
    public string?       Notes        { get; private set; }
    public Guid          RecordedBy   { get; private set; }

    private Payment() { }

    public static Payment Create(
        Guid          schoolId,
        Guid          invoiceId,
        Guid          parentUserId,
        Money         amount,
        PaymentMethod method,
        string        reference,
        Guid          recordedBy,
        string?       notes = null)
    {
        if (string.IsNullOrWhiteSpace(reference))
            throw new ArgumentException("Payment reference is required.");

        return new Payment
        {
            PaymentId    = Guid.NewGuid(),
            SchoolId     = schoolId,
            InvoiceId    = invoiceId,
            ParentUserId = parentUserId,
            Amount       = amount,
            PaymentDate  = DateTime.UtcNow,
            Method       = method,
            Reference    = reference.Trim(),
            Status       = PaymentStatus.Pending,
            Notes        = notes?.Trim(),
            RecordedBy   = recordedBy
        };
    }

    public static Payment Reconstitute(
        Guid          paymentId,
        Guid          schoolId,
        Guid          invoiceId,
        Guid          parentUserId,
        Money         amount,
        DateTime      paymentDate,
        PaymentMethod method,
        string        reference,
        PaymentStatus status,
        string?       notes,
        Guid          recordedBy) => new()
    {
        PaymentId    = paymentId,
        SchoolId     = schoolId,
        InvoiceId    = invoiceId,
        ParentUserId = parentUserId,
        Amount       = amount,
        PaymentDate  = paymentDate,
        Method       = method,
        Reference    = reference,
        Status       = status,
        Notes        = notes,
        RecordedBy   = recordedBy
    };

    public void Confirm() => Status = PaymentStatus.Confirmed;
    public void Reject()  => Status = PaymentStatus.Rejected;
}
