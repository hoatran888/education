using SchoolSystem.Domain.Models.Base;
using SchoolSystem.Domain.Models.Enums;

namespace SchoolSystem.Domain.Models.Entities;

public class Enrollment : EntityBase
{
    private readonly List<Grade> _grades = new();

    public Guid             EnrollmentId  { get; private set; }
    public Guid             CourseId      { get; private set; }
    public Guid             StudentUserId { get; private set; }
    public Guid             SchoolId      { get; private set; }
    public DateTime         EnrolledDate  { get; private set; }
    public EnrollmentStatus Status        { get; private set; }
    public Guid             EnrolledBy    { get; private set; }
    public DateTime?        DroppedDate   { get; private set; }
    public string?          DropReason    { get; private set; }

    public IReadOnlyList<Grade> Grades => _grades.AsReadOnly();

    private Enrollment() { }

    public static Enrollment Create(
        Guid courseId,
        Guid studentUserId,
        Guid schoolId,
        Guid enrolledBy)
    {
        return new Enrollment
        {
            EnrollmentId  = Guid.NewGuid(),
            CourseId      = courseId,
            StudentUserId = studentUserId,
            SchoolId      = schoolId,
            EnrolledDate  = DateTime.UtcNow,
            Status        = EnrollmentStatus.Active,
            EnrolledBy    = enrolledBy
        };
    }

    public static Enrollment Reconstitute(
        Guid             enrollmentId,
        Guid             courseId,
        Guid             studentUserId,
        Guid             schoolId,
        DateTime         enrolledDate,
        EnrollmentStatus status,
        Guid             enrolledBy,
        DateTime?        droppedDate,
        string?          dropReason) => new()
    {
        EnrollmentId  = enrollmentId,
        CourseId      = courseId,
        StudentUserId = studentUserId,
        SchoolId      = schoolId,
        EnrolledDate  = enrolledDate,
        Status        = status,
        EnrolledBy    = enrolledBy,
        DroppedDate   = droppedDate,
        DropReason    = dropReason
    };

    public void Drop(string reason)
    {
        if (Status == EnrollmentStatus.Dropped)
            throw new InvalidOperationException("Enrollment is already dropped.");
        if (Status == EnrollmentStatus.Completed)
            throw new InvalidOperationException("Cannot drop a completed enrollment.");

        Status      = EnrollmentStatus.Dropped;
        DroppedDate = DateTime.UtcNow;
        DropReason  = reason;
    }

    public void Complete()
    {
        if (Status != EnrollmentStatus.Active)
            throw new InvalidOperationException("Only active enrollments can be completed.");
        Status = EnrollmentStatus.Completed;
    }

    public void AddGrade(Grade grade) => _grades.Add(grade);
}
