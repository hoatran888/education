namespace SchoolSystem.Infrastructure.Data.LinqEntities;

public class EnrollmentEntity
{
    public Guid      EnrollmentId  { get; set; }
    public Guid      CourseId      { get; set; }
    public Guid      StudentUserId { get; set; }
    public Guid      SchoolId      { get; set; }
    public DateTime  EnrolledDate  { get; set; }
    public string    Status        { get; set; } = string.Empty;
    public Guid      EnrolledBy    { get; set; }
    public DateTime? DroppedDate   { get; set; }
    public string?   DropReason    { get; set; }
}
