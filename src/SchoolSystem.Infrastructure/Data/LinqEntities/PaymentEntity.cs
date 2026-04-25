namespace SchoolSystem.Infrastructure.Data.LinqEntities;

public class PaymentEntity
{
    public Guid     PaymentId   { get; set; }
    public Guid     SchoolId    { get; set; }
    public Guid     InvoiceId   { get; set; }
    public Guid     ParentUserId { get; set; }
    public decimal  Amount      { get; set; }
    public string   Currency    { get; set; } = string.Empty;
    public DateTime PaymentDate { get; set; }
    public int      Method      { get; set; }
    public string   Reference   { get; set; } = string.Empty;
    public int      Status      { get; set; }
    public string?  Notes       { get; set; }
    public Guid     RecordedBy  { get; set; }
}
