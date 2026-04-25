namespace SchoolSystem.Domain.Models.ValueObjects;

public record DateRange
{
    public DateOnly Start { get; init; }
    public DateOnly End   { get; init; }

    public DateRange(DateOnly start, DateOnly end)
    {
        if (end <= start)
            throw new ArgumentException("End date must be after start date.");
        Start = start;
        End   = end;
    }

    public bool Overlaps(DateRange other) =>
        Start < other.End && End > other.Start;

    public bool Contains(DateOnly date) =>
        date >= Start && date <= End;

    public int TotalDays => End.DayNumber - Start.DayNumber;

    public override string ToString() => $"{Start:yyyy-MM-dd} to {End:yyyy-MM-dd}";
}
