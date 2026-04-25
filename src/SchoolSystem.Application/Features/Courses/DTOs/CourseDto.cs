namespace SchoolSystem.Application.Features.Courses.DTOs;

public record CourseDto(
    Guid      CourseId,
    Guid      SchoolId,
    Guid      AcademicYearId,
    string    Name,
    string?   Description,
    int       GradeLevel,
    int       Credits,
    int       MaxStudents,
    int       ActiveEnrollments,
    Guid?     TeacherUserId,
    string    Status,
    DateOnly  DurationStart,
    DateOnly  DurationEnd,
    DateTime  CreatedAt);
