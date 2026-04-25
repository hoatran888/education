namespace SchoolSystem.Infrastructure.Data.LinqEntities;

public class CourseEntity
{
    public Guid    CourseId       { get; set; }
    public Guid    SchoolId       { get; set; }
    public Guid    AcademicYearId { get; set; }
    public string  Name           { get; set; } = string.Empty;
    public string? Description    { get; set; }
    public int     GradeLevel     { get; set; }
    public int     Credits        { get; set; }
    public int     MaxStudents    { get; set; }
    public Guid?   TeacherUserId  { get; set; }
    public string  Status         { get; set; } = string.Empty;
    public DateTime StartDate     { get; set; }
    public DateTime EndDate       { get; set; }
    public DateTime CreatedAt     { get; set; }
    public Guid    CreatedBy      { get; set; }

    public ICollection<EnrollmentEntity> Enrollments { get; set; } = [];
}
