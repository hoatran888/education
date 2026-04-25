namespace SchoolSystem.Infrastructure.Data.LinqEntities;

public class FeeStructureEntity
{
    public Guid      FeeStructureId { get; set; }
    public Guid      SchoolId       { get; set; }
    public Guid?     CourseId       { get; set; }
    public string    Name           { get; set; } = string.Empty;
    public decimal   Amount         { get; set; }
    public string    Currency       { get; set; } = string.Empty;
    public bool      IsRecurring    { get; set; }
    public int?      DueDays        { get; set; }
    public DateTime? FixedDueDate   { get; set; }
    public bool      IsActive       { get; set; }
}
