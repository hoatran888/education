namespace SchoolSystem.Application.Features.Users.DTOs;

public record UserDto(
    Guid          UserId,
    Guid          SchoolId,
    string        FirstName,
    string        LastName,
    string        FullName,
    string        Email,
    string        Phone,
    string        Sex,
    string        Street,
    string        City,
    string        State,
    string        ZipCode,
    string        Country,
    string?       PhotoUrl,
    bool          IsActive,
    DateTime      CreatedAt,
    IReadOnlyList<string> Roles);
