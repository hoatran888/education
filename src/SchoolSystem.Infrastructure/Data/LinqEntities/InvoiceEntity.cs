namespace SchoolSystem.Infrastructure.Data.LinqEntities;

public class InvoiceEntity
{
    public Guid      InvoiceId     { get; set; }
    public Guid      SchoolId      { get; set; }
    public Guid      StudentUserId { get; set; }
    public Guid      ParentUserId  { get; set; }
    public Guid      AcademicYearId { get; set; }
    public decimal   TotalAmount   { get; set; }
    public decimal   PaidAmount    { get; set; }
    public string    Currency      { get; set; } = string.Empty;
    public string    Status        { get; set; } = string.Empty;
    public DateTime  DueDate       { get; set; }
    public DateTime  IssuedDate    { get; set; }
    public DateTime? PaidDate      { get; set; }
    public string?   Notes         { get; set; }

    public ICollection<InvoiceItemEntity> Items { get; set; } = [];
}
