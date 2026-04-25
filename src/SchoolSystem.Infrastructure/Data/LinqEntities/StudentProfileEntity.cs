namespace SchoolSystem.Infrastructure.Data.LinqEntities;

public class StudentProfileEntity
{
    public Guid     StudentProfileId { get; set; }
    public Guid     UserId           { get; set; }
    public Guid     SchoolId         { get; set; }
    public DateTime BirthDate        { get; set; }
    public int      GradeLevel       { get; set; }
    public DateTime AdmissionDate    { get; set; }
    public bool     IsActive         { get; set; }
}
