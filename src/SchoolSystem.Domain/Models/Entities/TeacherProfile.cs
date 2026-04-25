namespace SchoolSystem.Domain.Models.Entities;

public class TeacherProfile
{
    public Guid     TeacherProfileId { get; private set; }
    public Guid     UserId           { get; private set; }
    public Guid     SchoolId         { get; private set; }
    public string   Degree           { get; private set; }
    public string   Specialization   { get; private set; }
    public DateTime HireDate         { get; private set; }
    public bool     IsActive         { get; private set; }

    public User? User { get; set; }

    private TeacherProfile() { }

    public static TeacherProfile Create(
        Guid   userId,
        Guid   schoolId,
        string degree,
        string specialization)
    {
        if (string.IsNullOrWhiteSpace(degree))
            throw new ArgumentException("Degree is required.");

        return new TeacherProfile
        {
            TeacherProfileId = Guid.NewGuid(),
            UserId           = userId,
            SchoolId         = schoolId,
            Degree           = degree.Trim(),
            Specialization   = specialization?.Trim() ?? string.Empty,
            HireDate         = DateTime.UtcNow,
            IsActive         = true
        };
    }

    public static TeacherProfile Reconstitute(
        Guid     teacherProfileId,
        Guid     userId,
        Guid     schoolId,
        string   degree,
        string   specialization,
        DateTime hireDate,
        bool     isActive) => new()
    {
        TeacherProfileId = teacherProfileId,
        UserId           = userId,
        SchoolId         = schoolId,
        Degree           = degree,
        Specialization   = specialization,
        HireDate         = hireDate,
        IsActive         = isActive
    };

    public void Update(string degree, string specialization)
    {
        if (string.IsNullOrWhiteSpace(degree))
            throw new ArgumentException("Degree is required.");
        Degree         = degree.Trim();
        Specialization = specialization?.Trim() ?? string.Empty;
    }

    public void Deactivate() => IsActive = false;
    public void Activate()   => IsActive = true;
}
