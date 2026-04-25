namespace SchoolSystem.Application.Features.Students.DTOs;

public record StudentDto(
    Guid      StudentProfileId,
    Guid      UserId,
    Guid      SchoolId,
    string    FullName,
    string    Email,
    DateOnly  BirthDate,
    int       Age,
    int       GradeLevel,
    DateTime  AdmissionDate,
    bool      IsActive);
