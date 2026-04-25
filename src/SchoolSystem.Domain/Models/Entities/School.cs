using SchoolSystem.Domain.Models.ValueObjects;

namespace SchoolSystem.Domain.Models.Entities;

public class School
{
    public Guid        SchoolId  { get; private set; }
    public string      Name      { get; private set; }
    public Address     Address   { get; private set; }
    public ContactInfo Contact   { get; private set; }
    public string?     LogoUrl   { get; private set; }
    public bool        IsActive  { get; private set; }
    public DateTime    CreatedAt { get; private set; }

    private School() { }

    public static School Create(
        string      name,
        Address     address,
        ContactInfo contact,
        string?     logoUrl = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("School name is required.");

        return new School
        {
            SchoolId  = Guid.NewGuid(),
            Name      = name.Trim(),
            Address   = address,
            Contact   = contact,
            LogoUrl   = logoUrl,
            IsActive  = true,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void UpdateInfo(string name, Address address, ContactInfo contact)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("School name is required.");
        Name    = name.Trim();
        Address = address;
        Contact = contact;
    }

    public static School Reconstitute(
        Guid        schoolId,
        string      name,
        Address     address,
        ContactInfo contact,
        string?     logoUrl,
        bool        isActive,
        DateTime    createdAt) => new()
    {
        SchoolId  = schoolId,
        Name      = name,
        Address   = address,
        Contact   = contact,
        LogoUrl   = logoUrl,
        IsActive  = isActive,
        CreatedAt = createdAt
    };

    public void UpdateLogo(string logoUrl) => LogoUrl = logoUrl;
    public void Deactivate()               => IsActive = false;
    public void Activate()                 => IsActive = true;
}
