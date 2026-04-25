namespace SchoolSystem.Application.Features.Schools.DTOs;

public record SchoolDto(
    Guid    SchoolId,
    string  Name,
    string  Street,
    string  City,
    string  State,
    string  ZipCode,
    string  Country,
    string  Email,
    string  Phone,
    string? LogoUrl,
    bool    IsActive,
    DateTime CreatedAt);
