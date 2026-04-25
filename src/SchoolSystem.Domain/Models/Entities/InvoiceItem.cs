using SchoolSystem.Domain.Models.ValueObjects;

namespace SchoolSystem.Domain.Models.Entities;

public class InvoiceItem
{
    public Guid   InvoiceItemId { get; private set; }
    public Guid   InvoiceId     { get; private set; }
    public Guid?  CourseId      { get; private set; }
    public string Description   { get; private set; }
    public Money  Amount        { get; private set; }

    private InvoiceItem() { }

    public static InvoiceItem Create(
        Guid    invoiceId,
        string  description,
        Money   amount,
        Guid?   courseId = null)
    {
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Description is required.");

        return new InvoiceItem
        {
            InvoiceItemId = Guid.NewGuid(),
            InvoiceId     = invoiceId,
            CourseId      = courseId,
            Description   = description.Trim(),
            Amount        = amount
        };
    }
}
