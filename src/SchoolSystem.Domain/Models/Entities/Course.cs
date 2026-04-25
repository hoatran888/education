using SchoolSystem.Domain.Models.Base;
using SchoolSystem.Domain.Models.Enums;
using SchoolSystem.Domain.Models.ValueObjects;

namespace SchoolSystem.Domain.Models.Entities;

public class Course : EntityBase
{
    private readonly List<Enrollment> _enrollments = new();
    private readonly List<Schedule>   _schedules   = new();

    public Guid         CourseId       { get; private set; }
    public Guid         SchoolId       { get; private set; }
    public Guid         AcademicYearId { get; private set; }
    public string       Name           { get; private set; }
    public string?      Description    { get; private set; }
    public int          GradeLevel     { get; private set; }
    public int          Credits        { get; private set; }
    public int          MaxStudents    { get; private set; }
    public Guid?        TeacherUserId  { get; private set; }
    public CourseStatus Status         { get; private set; }
    public DateRange    Duration       { get; private set; }
    public DateTime     CreatedAt      { get; private set; }
    public Guid         CreatedBy      { get; private set; }

    public IReadOnlyList<Enrollment> Enrollments => _enrollments.AsReadOnly();
    public IReadOnlyList<Schedule>   Schedules   => _schedules.AsReadOnly();

    public int ActiveEnrollmentCount =>
        _enrollments.Count(e => e.Status == EnrollmentStatus.Active);

    public bool HasCapacity => ActiveEnrollmentCount < MaxStudents;

    private Course() { }

    public static Course Create(
        Guid      schoolId,
        Guid      academicYearId,
        string    name,
        string?   description,
        int       gradeLevel,
        int       credits,
        int       maxStudents,
        DateRange duration,
        Guid      createdBy)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Course name is required.");
        if (maxStudents < 1)
            throw new ArgumentException("Max students must be at least 1.");

        return new Course
        {
            CourseId       = Guid.NewGuid(),
            SchoolId       = schoolId,
            AcademicYearId = academicYearId,
            Name           = name.Trim(),
            Description    = description?.Trim(),
            GradeLevel     = gradeLevel,
            Credits        = credits,
            MaxStudents    = maxStudents,
            Duration       = duration,
            Status         = CourseStatus.Draft,
            CreatedAt      = DateTime.UtcNow,
            CreatedBy      = createdBy
        };
    }

    public static Course Reconstitute(
        Guid         courseId,
        Guid         schoolId,
        Guid         academicYearId,
        string       name,
        string?      description,
        int          gradeLevel,
        int          credits,
        int          maxStudents,
        Guid?        teacherUserId,
        CourseStatus status,
        DateRange    duration,
        DateTime     createdAt,
        Guid         createdBy) => new()
    {
        CourseId       = courseId,
        SchoolId       = schoolId,
        AcademicYearId = academicYearId,
        Name           = name,
        Description    = description,
        GradeLevel     = gradeLevel,
        Credits        = credits,
        MaxStudents    = maxStudents,
        TeacherUserId  = teacherUserId,
        Status         = status,
        Duration       = duration,
        CreatedAt      = createdAt,
        CreatedBy      = createdBy
    };

    public void AssignTeacher(Guid teacherUserId)
    {
        if (Status == CourseStatus.Closed)
            throw new InvalidOperationException("Cannot reassign teacher on a closed course.");
        TeacherUserId = teacherUserId;
    }

    public void Open()
    {
        if (TeacherUserId is null)
            throw new InvalidOperationException("A teacher must be assigned before opening a course.");
        if (Status != CourseStatus.Draft)
            throw new InvalidOperationException("Only draft courses can be opened.");
        Status = CourseStatus.Open;
    }

    public Enrollment EnrollStudent(Guid studentUserId, Guid enrolledBy)
    {
        if (Status != CourseStatus.Open)
            throw new InvalidOperationException("Course is not accepting enrollments.");
        if (!HasCapacity)
            throw new InvalidOperationException("Course has reached maximum capacity.");
        if (_enrollments.Any(e =>
                e.StudentUserId == studentUserId &&
                e.Status        != EnrollmentStatus.Dropped))
            throw new InvalidOperationException("Student is already enrolled in this course.");

        var enrollment = Enrollment.Create(CourseId, studentUserId, SchoolId, enrolledBy);
        _enrollments.Add(enrollment);
        return enrollment;
    }

    public void Close()
    {
        if (Status == CourseStatus.Closed)
            throw new InvalidOperationException("Course is already closed.");
        Status = CourseStatus.Closed;
    }

    public void Update(string name, string? description, int maxStudents)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Course name is required.");
        Name        = name.Trim();
        Description = description?.Trim();
        MaxStudents = maxStudents;
    }

    public void AddSchedule(Schedule schedule) => _schedules.Add(schedule);
}
