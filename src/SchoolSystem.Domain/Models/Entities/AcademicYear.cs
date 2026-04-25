using SchoolSystem.Domain.Models.ValueObjects;

namespace SchoolSystem.Domain.Models.Entities;

public class AcademicYear
{
    private readonly List<Term> _terms = new();

    public Guid     AcademicYearId { get; private set; }
    public Guid     SchoolId       { get; private set; }
    public string   Name           { get; private set; }
    public DateOnly StartDate      { get; private set; }
    public DateOnly EndDate        { get; private set; }
    public bool     IsActive       { get; private set; }

    public IReadOnlyList<Term> Terms => _terms.AsReadOnly();

    public DateRange Period => new(StartDate, EndDate);

    private AcademicYear() { }

    public static AcademicYear Create(
        Guid     schoolId,
        string   name,
        DateOnly startDate,
        DateOnly endDate)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Academic year name is required.");
        if (endDate <= startDate)
            throw new ArgumentException("End date must be after start date.");

        return new AcademicYear
        {
            AcademicYearId = Guid.NewGuid(),
            SchoolId       = schoolId,
            Name           = name.Trim(),
            StartDate      = startDate,
            EndDate        = endDate,
            IsActive       = true
        };
    }

    public void AddTerm(Term term)   => _terms.Add(term);
    public void Activate()           => IsActive = true;
    public void Deactivate()         => IsActive = false;
}
