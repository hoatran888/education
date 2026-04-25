namespace SchoolSystem.Infrastructure.Data.LinqEntities;

public class GradePeriodEntity
{
    public Guid     GradePeriodId { get; set; }
    public Guid     TermId        { get; set; }
    public Guid     SchoolId      { get; set; }
    public string   Name          { get; set; } = string.Empty;
    public int      Type          { get; set; }
    public DateTime StartDate     { get; set; }
    public DateTime EndDate       { get; set; }
}
