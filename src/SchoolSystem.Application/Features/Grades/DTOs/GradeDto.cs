namespace SchoolSystem.Application.Features.Grades.DTOs;

public record GradeDto(
    Guid      GradeId,
    Guid      EnrollmentId,
    Guid      GradePeriodId,
    Guid      SchoolId,
    Guid      TeacherUserId,
    decimal   Score,
    string    LetterGrade,
    string?   Comment,
    bool      IsPublished,
    DateTime? PublishedAt,
    DateTime  GradedAt,
    string?   PeriodName,
    string?   CourseName);
