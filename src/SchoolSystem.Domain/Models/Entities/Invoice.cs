using SchoolSystem.Domain.Models.Base;
using SchoolSystem.Domain.Models.Enums;
using SchoolSystem.Domain.Models.ValueObjects;

namespace SchoolSystem.Domain.Models.Entities;

public class Invoice : EntityBase
{
    private readonly List<InvoiceItem> _items    = new();
    private readonly List<Payment>     _payments = new();

    public Guid          InvoiceId      { get; private set; }
    public Guid          SchoolId       { get; private set; }
    public Guid          StudentUserId  { get; private set; }
    public Guid          ParentUserId   { get; private set; }
    public Guid          AcademicYearId { get; private set; }
    public Money         TotalAmount    { get; private set; }
    public Money         PaidAmount     { get; private set; }
    public InvoiceStatus Status         { get; private set; }
    public DateOnly      DueDate        { get; private set; }
    public DateTime      IssuedDate     { get; private set; }
    public DateTime?     PaidDate       { get; private set; }
    public string?       Notes          { get; private set; }

    public IReadOnlyList<InvoiceItem> Items    => _items.AsReadOnly();
    public IReadOnlyList<Payment>     Payments => _payments.AsReadOnly();

    public Money OutstandingBalance =>
        new(Math.Max(0, TotalAmount.Amount - PaidAmount.Amount), TotalAmount.Currency);

    public bool IsFullyPaid => PaidAmount.Amount >= TotalAmount.Amount;

    private Invoice() { }

    public static Invoice Create(
        Guid     schoolId,
        Guid     studentUserId,
        Guid     parentUserId,
        Guid     academicYearId,
        Money    totalAmount,
        DateOnly dueDate)
    {
        return new Invoice
        {
            InvoiceId      = Guid.NewGuid(),
            SchoolId       = schoolId,
            StudentUserId  = studentUserId,
            ParentUserId   = parentUserId,
            AcademicYearId = academicYearId,
            TotalAmount    = totalAmount,
            PaidAmount     = Money.Zero(totalAmount.Currency),
            Status         = InvoiceStatus.Draft,
            DueDate        = dueDate,
            IssuedDate     = DateTime.UtcNow
        };
    }

    public static Invoice Reconstitute(
        Guid          invoiceId,
        Guid          schoolId,
        Guid          studentUserId,
        Guid          parentUserId,
        Guid          academicYearId,
        Money         totalAmount,
        Money         paidAmount,
        InvoiceStatus status,
        DateOnly      dueDate,
        DateTime      issuedDate,
        DateTime?     paidDate,
        string?       notes) => new()
    {
        InvoiceId      = invoiceId,
        SchoolId       = schoolId,
        StudentUserId  = studentUserId,
        ParentUserId   = parentUserId,
        AcademicYearId = academicYearId,
        TotalAmount    = totalAmount,
        PaidAmount     = paidAmount,
        Status         = status,
        DueDate        = dueDate,
        IssuedDate     = issuedDate,
        PaidDate       = paidDate,
        Notes          = notes
    };

    public void Send()
    {
        if (Status != InvoiceStatus.Draft)
            throw new InvalidOperationException("Only draft invoices can be sent.");
        Status = InvoiceStatus.Sent;
    }

    public void AddItem(InvoiceItem item)
    {
        _items.Add(item);
        TotalAmount = new Money(_items.Sum(i => i.Amount.Amount), TotalAmount.Currency);
    }

    public void ApplyPayment(Payment payment)
    {
        if (Status == InvoiceStatus.Waived)
            throw new InvalidOperationException("Cannot apply payment to a waived invoice.");
        if (Status == InvoiceStatus.Paid)
            throw new InvalidOperationException("Invoice is already fully paid.");
        if (payment.Amount.Currency != TotalAmount.Currency)
            throw new InvalidOperationException("Payment currency does not match invoice currency.");

        _payments.Add(payment);
        PaidAmount = PaidAmount.Add(payment.Amount);

        if (IsFullyPaid)
        {
            Status   = InvoiceStatus.Paid;
            PaidDate = DateTime.UtcNow;
        }
    }

    public void MarkOverdue()
    {
        if (Status != InvoiceStatus.Sent) return;
        if (DateOnly.FromDateTime(DateTime.UtcNow) <= DueDate) return;
        Status = InvoiceStatus.Overdue;
    }

    public void Waive(string reason, Guid approvedBy)
    {
        if (Status == InvoiceStatus.Paid)
            throw new InvalidOperationException("Cannot waive a fully paid invoice.");
        Status = InvoiceStatus.Waived;
        Notes  = $"Waived by {approvedBy}: {reason}";
    }
}
