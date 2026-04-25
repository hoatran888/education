namespace SchoolSystem.Infrastructure.Data.LinqEntities;

public class AcademicYearEntity
{
    public Guid     AcademicYearId { get; set; }
    public Guid     SchoolId       { get; set; }
    public string   Name           { get; set; } = string.Empty;
    public DateTime StartDate      { get; set; }
    public DateTime EndDate        { get; set; }
    public bool     IsActive       { get; set; }
}
