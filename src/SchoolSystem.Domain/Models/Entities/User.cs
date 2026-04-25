using SchoolSystem.Domain.Models.Base;
using SchoolSystem.Domain.Models.Enums;
using SchoolSystem.Domain.Models.ValueObjects;

namespace SchoolSystem.Domain.Models.Entities;

public class User : EntityBase
{
    private readonly List<UserRole> _roles = new();

    public Guid     UserId      { get; private set; }
    public Guid     SchoolId    { get; private set; }
    public string   B2CObjectId { get; private set; }
    public string   FirstName   { get; private set; }
    public string   LastName    { get; private set; }
    public Sex      Sex         { get; private set; }
    public string   Email       { get; private set; }
    public string   Phone       { get; private set; }
    public Address  Address     { get; private set; }
    public string?  PhotoUrl    { get; private set; }
    public bool     IsActive    { get; private set; }
    public DateTime CreatedAt   { get; private set; }

    public string FullName => $"{FirstName} {LastName}";

    public IReadOnlyList<UserRole> Roles => _roles.AsReadOnly();

    private User() { }

    public static User Create(
        Guid    schoolId,
        string  b2cObjectId,
        string  firstName,
        string  lastName,
        Sex     sex,
        string  email,
        string  phone,
        Address address)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("First name is required.");
        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("Last name is required.");
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email is required.");

        return new User
        {
            UserId      = Guid.NewGuid(),
            SchoolId    = schoolId,
            B2CObjectId = b2cObjectId,
            FirstName   = firstName.Trim(),
            LastName    = lastName.Trim(),
            Sex         = sex,
            Email       = email.Trim().ToLowerInvariant(),
            Phone       = phone?.Trim() ?? string.Empty,
            Address     = address,
            IsActive    = true,
            CreatedAt   = DateTime.UtcNow
        };
    }

    public void AssignRole(UserRole role)
    {
        if (!_roles.Contains(role))
            _roles.Add(role);
    }

    public void RemoveRole(UserRole role) => _roles.Remove(role);

    public bool IsInRole(UserRole role) => _roles.Contains(role);

    public void UpdateProfile(
        string  firstName,
        string  lastName,
        string  phone,
        Address address)
    {
        FirstName = firstName.Trim();
        LastName  = lastName.Trim();
        Phone     = phone?.Trim() ?? string.Empty;
        Address   = address;
    }

    public static User Reconstitute(
        Guid            userId,
        Guid            schoolId,
        string          b2cObjectId,
        string          firstName,
        string          lastName,
        Sex             sex,
        string          email,
        string          phone,
        Address         address,
        string?         photoUrl,
        bool            isActive,
        DateTime        createdAt,
        IEnumerable<UserRole> roles)
    {
        var user = new User
        {
            UserId      = userId,
            SchoolId    = schoolId,
            B2CObjectId = b2cObjectId,
            FirstName   = firstName,
            LastName    = lastName,
            Sex         = sex,
            Email       = email,
            Phone       = phone,
            Address     = address,
            PhotoUrl    = photoUrl,
            IsActive    = isActive,
            CreatedAt   = createdAt
        };
        foreach (var role in roles)
            user._roles.Add(role);
        return user;
    }

    public void UpdatePhoto(string photoUrl) => PhotoUrl = photoUrl;
    public void Deactivate()                 => IsActive = false;
    public void Activate()                   => IsActive = true;
}
