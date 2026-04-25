namespace SchoolSystem.Application.Features.Enrollments.DTOs;

public record EnrollmentDto(
    Guid      EnrollmentId,
    Guid      CourseId,
    Guid      StudentUserId,
    Guid      SchoolId,
    DateTime  EnrolledDate,
    string    Status,
    DateTime? DroppedDate,
    string?   DropReason);
