using SchoolSystem.Domain.Models.ValueObjects;

namespace SchoolSystem.Domain.Models.Entities;

public class FeeStructure
{
    public Guid     FeeStructureId { get; private set; }
    public Guid     SchoolId       { get; private set; }
    public Guid?    CourseId       { get; private set; }
    public string   Name           { get; private set; }
    public Money    Amount         { get; private set; }
    public bool     IsRecurring    { get; private set; }
    public int?     DueDays        { get; private set; }
    public DateOnly? FixedDueDate  { get; private set; }
    public bool     IsActive       { get; private set; }

    private FeeStructure() { }

    public static FeeStructure Create(
        Guid      schoolId,
        string    name,
        Money     amount,
        Guid?     courseId     = null,
        bool      isRecurring  = false,
        int?      dueDays      = 30,
        DateOnly? fixedDueDate = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Fee structure name is required.");

        return new FeeStructure
        {
            FeeStructureId = Guid.NewGuid(),
            SchoolId       = schoolId,
            CourseId       = courseId,
            Name           = name.Trim(),
            Amount         = amount,
            IsRecurring    = isRecurring,
            DueDays        = dueDays,
            FixedDueDate   = fixedDueDate,
            IsActive       = true
        };
    }

    public DateOnly CalculateDueDate() =>
        FixedDueDate ?? DateOnly.FromDateTime(DateTime.UtcNow).AddDays(DueDays ?? 30);

    public void Deactivate() => IsActive = false;
    public void Activate()   => IsActive = true;
}
