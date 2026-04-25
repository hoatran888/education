using SchoolSystem.Domain.Models.ValueObjects;

namespace SchoolSystem.Domain.Models.Entities;

public class Term
{
    private readonly List<GradePeriod> _gradePeriods = new();

    public Guid     TermId         { get; private set; }
    public Guid     AcademicYearId { get; private set; }
    public Guid     SchoolId       { get; private set; }
    public string   Name           { get; private set; }
    public DateOnly StartDate      { get; private set; }
    public DateOnly EndDate        { get; private set; }

    public AcademicYear? AcademicYear { get; set; }
    public IReadOnlyList<GradePeriod> GradePeriods => _gradePeriods.AsReadOnly();

    public DateRange Period => new(StartDate, EndDate);

    private Term() { }

    public static Term Create(
        Guid     academicYearId,
        Guid     schoolId,
        string   name,
        DateOnly startDate,
        DateOnly endDate)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Term name is required.");
        if (endDate <= startDate)
            throw new ArgumentException("End date must be after start date.");

        return new Term
        {
            TermId         = Guid.NewGuid(),
            AcademicYearId = academicYearId,
            SchoolId       = schoolId,
            Name           = name.Trim(),
            StartDate      = startDate,
            EndDate        = endDate
        };
    }

    public void AddGradePeriod(GradePeriod period) => _gradePeriods.Add(period);
}
