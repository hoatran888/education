namespace SchoolSystem.Application.Features.Teachers.DTOs;

public record TeacherDto(
    Guid     TeacherProfileId,
    Guid     UserId,
    Guid     SchoolId,
    string   FullName,
    string   Email,
    string   Degree,
    string   Specialization,
    DateTime HireDate,
    bool     IsActive);
