namespace SchoolSystem.Infrastructure.Data.LinqEntities;

public class InvoiceItemEntity
{
    public Guid    InvoiceItemId { get; set; }
    public Guid    InvoiceId     { get; set; }
    public Guid?   CourseId      { get; set; }
    public string  Description   { get; set; } = string.Empty;
    public decimal Amount        { get; set; }
    public string  Currency      { get; set; } = string.Empty;
}
