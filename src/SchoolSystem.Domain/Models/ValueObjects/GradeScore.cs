using SchoolSystem.Domain.Models.Enums;

namespace SchoolSystem.Domain.Models.ValueObjects;

public record GradeScore
{
    public decimal Value { get; init; }

    public GradeScore(decimal value)
    {
        if (value < 0 || value > 100)
            throw new ArgumentOutOfRangeException(nameof(value),
                "Score must be between 0 and 100.");
        Value = Math.Round(value, 2);
    }

    public LetterGrade ToLetterGrade() => Value switch
    {
        >= 90 => LetterGrade.A,
        >= 80 => LetterGrade.B,
        >= 70 => LetterGrade.C,
        >= 60 => LetterGrade.D,
        _     => LetterGrade.F
    };

    public override string ToString() => $"{Value:F2}%";
}
