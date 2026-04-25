using SchoolSystem.Domain.Models.Enums;

namespace SchoolSystem.Domain.Models.Entities;

public class HRProfile
{
    public Guid     HRProfileId { get; private set; }
    public Guid     UserId      { get; private set; }
    public Guid     SchoolId    { get; private set; }
    public HRType   HRType      { get; private set; }
    public string?  Department  { get; private set; }
    public DateTime HireDate    { get; private set; }
    public bool     IsActive    { get; private set; }

    public User? User { get; set; }

    private HRProfile() { }

    public static HRProfile Create(
        Guid    userId,
        Guid    schoolId,
        HRType  hrType,
        string? department = null)
    {
        return new HRProfile
        {
            HRProfileId = Guid.NewGuid(),
            UserId      = userId,
            SchoolId    = schoolId,
            HRType      = hrType,
            Department  = department?.Trim(),
            HireDate    = DateTime.UtcNow,
            IsActive    = true
        };
    }

    public void Update(HRType hrType, string? department)
    {
        HRType     = hrType;
        Department = department?.Trim();
    }

    public void Deactivate() => IsActive = false;
    public void Activate()   => IsActive = true;
}
