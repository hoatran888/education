using SchoolSystem.Domain.Models.Base;
using SchoolSystem.Domain.Models.Enums;
using SchoolSystem.Domain.Models.ValueObjects;

namespace SchoolSystem.Domain.Models.Entities;

public class Grade : EntityBase
{
    public Guid        GradeId       { get; private set; }
    public Guid        EnrollmentId  { get; private set; }
    public Guid        GradePeriodId { get; private set; }
    public Guid        SchoolId      { get; private set; }
    public Guid        TeacherUserId { get; private set; }
    public GradeScore  Score         { get; private set; }
    public LetterGrade LetterGrade   { get; private set; }
    public string?     Comment       { get; private set; }
    public bool        IsPublished   { get; private set; }
    public DateTime?   PublishedAt   { get; private set; }
    public DateTime    GradedAt      { get; private set; }

    public string? PeriodName { get; set; }
    public string? CourseName { get; set; }

    private Grade() { }

    public static Grade Create(
        Enrollment  enrollment,
        GradePeriod period,
        Guid        teacherUserId,
        decimal     score,
        string?     comment)
    {
        if (enrollment.Status == EnrollmentStatus.Dropped)
            throw new InvalidOperationException("Cannot grade a dropped student.");
        if (!period.IsActive)
            throw new InvalidOperationException("Grade period is not currently active.");

        var gradeScore = new GradeScore(score);

        return new Grade
        {
            GradeId       = Guid.NewGuid(),
            EnrollmentId  = enrollment.EnrollmentId,
            GradePeriodId = period.GradePeriodId,
            SchoolId      = enrollment.SchoolId,
            TeacherUserId = teacherUserId,
            Score         = gradeScore,
            LetterGrade   = gradeScore.ToLetterGrade(),
            Comment       = comment?.Trim(),
            IsPublished   = false,
            GradedAt      = DateTime.UtcNow
        };
    }

    public static Grade Reconstitute(
        Guid        gradeId,
        Guid        enrollmentId,
        Guid        gradePeriodId,
        Guid        schoolId,
        Guid        teacherUserId,
        decimal     score,
        LetterGrade letterGrade,
        string?     comment,
        bool        isPublished,
        DateTime?   publishedAt,
        DateTime    gradedAt) => new()
    {
        GradeId       = gradeId,
        EnrollmentId  = enrollmentId,
        GradePeriodId = gradePeriodId,
        SchoolId      = schoolId,
        TeacherUserId = teacherUserId,
        Score         = new GradeScore(score),
        LetterGrade   = letterGrade,
        Comment       = comment,
        IsPublished   = isPublished,
        PublishedAt   = publishedAt,
        GradedAt      = gradedAt
    };

    public void UpdateScore(decimal newScore, string? newComment)
    {
        if (IsPublished)
            throw new InvalidOperationException("Published grades cannot be edited. Unpublish first.");

        Score       = new GradeScore(newScore);
        LetterGrade = Score.ToLetterGrade();
        Comment     = newComment?.Trim();
    }

    public void Publish()
    {
        if (IsPublished)
            throw new InvalidOperationException("Grade is already published.");
        IsPublished = true;
        PublishedAt = DateTime.UtcNow;
    }

    public void Unpublish()
    {
        IsPublished = false;
        PublishedAt = null;
    }
}
