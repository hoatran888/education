namespace SchoolSystem.Infrastructure.Data.LinqEntities;

public class GradeEntity
{
    public Guid      GradeId       { get; set; }
    public Guid      EnrollmentId  { get; set; }
    public Guid      GradePeriodId { get; set; }
    public Guid      SchoolId      { get; set; }
    public Guid      TeacherUserId { get; set; }
    public decimal   Score         { get; set; }
    public string    LetterGrade   { get; set; } = string.Empty;
    public string?   Comment       { get; set; }
    public bool      IsPublished   { get; set; }
    public DateTime? PublishedAt   { get; set; }
    public DateTime  GradedAt      { get; set; }
}
