namespace SchoolSystem.Infrastructure.Data.LinqEntities;

public class TermEntity
{
    public Guid     TermId         { get; set; }
    public Guid     AcademicYearId { get; set; }
    public Guid     SchoolId       { get; set; }
    public string   Name           { get; set; } = string.Empty;
    public DateTime StartDate      { get; set; }
    public DateTime EndDate        { get; set; }
}
