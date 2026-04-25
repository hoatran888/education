namespace SchoolSystem.Domain.Models.Entities;

public class StudentProfile
{
    private readonly List<StudentParent> _parents = new();

    public Guid      StudentProfileId { get; private set; }
    public Guid      UserId           { get; private set; }
    public Guid      SchoolId         { get; private set; }
    public DateOnly  BirthDate        { get; private set; }
    public int       GradeLevel       { get; private set; }
    public DateTime  AdmissionDate    { get; private set; }
    public bool      IsActive         { get; private set; }

    public User? User { get; set; }
    public IReadOnlyList<StudentParent> Parents => _parents.AsReadOnly();

    public int Age =>
        DateTime.UtcNow.Year - BirthDate.Year -
        (DateTime.UtcNow.DayOfYear < BirthDate.DayOfYear ? 1 : 0);

    private StudentProfile() { }

    public static StudentProfile Create(
        Guid     userId,
        Guid     schoolId,
        DateOnly birthDate,
        int      gradeLevel)
    {
        if (gradeLevel < 1 || gradeLevel > 13)
            throw new ArgumentException("Grade level must be between 1 and 13.");

        return new StudentProfile
        {
            StudentProfileId = Guid.NewGuid(),
            UserId           = userId,
            SchoolId         = schoolId,
            BirthDate        = birthDate,
            GradeLevel       = gradeLevel,
            AdmissionDate    = DateTime.UtcNow,
            IsActive         = true
        };
    }

    public static StudentProfile Reconstitute(
        Guid     studentProfileId,
        Guid     userId,
        Guid     schoolId,
        DateOnly birthDate,
        int      gradeLevel,
        DateTime admissionDate,
        bool     isActive) => new()
    {
        StudentProfileId = studentProfileId,
        UserId           = userId,
        SchoolId         = schoolId,
        BirthDate        = birthDate,
        GradeLevel       = gradeLevel,
        AdmissionDate    = admissionDate,
        IsActive         = isActive
    };

    public void UpdateGradeLevel(int gradeLevel)
    {
        if (gradeLevel < 1 || gradeLevel > 13)
            throw new ArgumentException("Grade level must be between 1 and 13.");
        GradeLevel = gradeLevel;
    }

    public void AddParent(StudentParent link) => _parents.Add(link);
    public void Deactivate()                  => IsActive = false;
    public void Activate()                    => IsActive = true;
}
