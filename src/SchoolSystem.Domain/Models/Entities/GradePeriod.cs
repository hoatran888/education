using SchoolSystem.Domain.Models.Enums;
using SchoolSystem.Domain.Models.ValueObjects;

namespace SchoolSystem.Domain.Models.Entities;

public class GradePeriod
{
    public Guid            GradePeriodId { get; private set; }
    public Guid            SchoolId      { get; private set; }
    public Guid            TermId        { get; private set; }
    public string          Name          { get; private set; }
    public GradePeriodType Type          { get; private set; }
    public DateOnly        StartDate     { get; private set; }
    public DateOnly        EndDate       { get; private set; }

    public Term? Term { get; set; }

    public bool IsActive =>
        DateOnly.FromDateTime(DateTime.UtcNow) >= StartDate &&
        DateOnly.FromDateTime(DateTime.UtcNow) <= EndDate;

    public DateRange Period => new(StartDate, EndDate);

    private GradePeriod() { }

    public static GradePeriod Reconstitute(
        Guid            gradePeriodId,
        Guid            schoolId,
        Guid            termId,
        string          name,
        GradePeriodType type,
        DateOnly        startDate,
        DateOnly        endDate) => new()
    {
        GradePeriodId = gradePeriodId,
        SchoolId      = schoolId,
        TermId        = termId,
        Name          = name,
        Type          = type,
        StartDate     = startDate,
        EndDate       = endDate
    };

    public static GradePeriod Create(
        Guid           termId,
        Guid           schoolId,
        string         name,
        GradePeriodType type,
        DateOnly       startDate,
        DateOnly       endDate)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Grade period name is required.");
        if (endDate <= startDate)
            throw new ArgumentException("End date must be after start date.");

        return new GradePeriod
        {
            GradePeriodId = Guid.NewGuid(),
            SchoolId      = schoolId,
            TermId        = termId,
            Name          = name.Trim(),
            Type          = type,
            StartDate     = startDate,
            EndDate       = endDate
        };
    }
}
