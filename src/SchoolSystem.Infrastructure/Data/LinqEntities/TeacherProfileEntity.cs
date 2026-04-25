namespace SchoolSystem.Infrastructure.Data.LinqEntities;

public class TeacherProfileEntity
{
    public Guid    TeacherProfileId { get; set; }
    public Guid    UserId           { get; set; }
    public Guid    SchoolId         { get; set; }
    public string  Degree           { get; set; } = string.Empty;
    public string  Specialization   { get; set; } = string.Empty;
    public DateTime HireDate        { get; set; }
    public bool    IsActive         { get; set; }
}
