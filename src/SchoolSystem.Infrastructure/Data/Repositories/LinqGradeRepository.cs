using Microsoft.EntityFrameworkCore;
using SchoolSystem.Domain.Models.Entities;
using SchoolSystem.Domain.Models.Enums;
using SchoolSystem.Infrastructure.Data.Context;
using SchoolSystem.Infrastructure.Data.Interfaces;
using SchoolSystem.Infrastructure.Data.LinqEntities;

namespace SchoolSystem.Infrastructure.Data.Repositories;

public class LinqGradeRepository
    : LinqRepositoryBase<GradeEntity, Grade>, IGradeRepository
{
    public LinqGradeRepository(SchoolDataContext context, Guid schoolId)
        : base(context, schoolId) { }

    public async Task<Grade?> GetByIdAsync(Guid gradeId, CancellationToken ct = default) =>
        await FirstOrDefaultAsync(
            Table.Where(g => g.SchoolId == _schoolId && g.GradeId == gradeId), ct);

    public async Task<IReadOnlyList<Grade>> GetByEnrollmentAsync(
        Guid enrollmentId, CancellationToken ct = default) =>
        await ToListAsync(
            Table.Where(g => g.SchoolId == _schoolId && g.EnrollmentId == enrollmentId)
                 .OrderBy(g => g.GradedAt), ct);

    public async Task<IReadOnlyList<Grade>> GetByStudentAsync(
        Guid studentUserId, CancellationToken ct = default)
    {
        var enrollmentIds = await _context.Enrollments
            .Where(e => e.SchoolId == _schoolId && e.StudentUserId == studentUserId)
            .Select(e => e.EnrollmentId)
            .ToListAsync(ct);

        return await ToListAsync(
            Table.Where(g => g.SchoolId == _schoolId && enrollmentIds.Contains(g.EnrollmentId))
                 .OrderBy(g => g.GradedAt), ct);
    }

    public async Task<IReadOnlyList<Grade>> GetByCourseAndPeriodAsync(
        Guid courseId, Guid gradePeriodId, CancellationToken ct = default)
    {
        var enrollmentIds = await _context.Enrollments
            .Where(e => e.SchoolId == _schoolId && e.CourseId == courseId)
            .Select(e => e.EnrollmentId)
            .ToListAsync(ct);

        return await ToListAsync(
            Table.Where(g => g.SchoolId      == _schoolId     &&
                             g.GradePeriodId  == gradePeriodId  &&
                             enrollmentIds.Contains(g.EnrollmentId))
                 .OrderBy(g => g.GradedAt), ct);
    }

    public async Task<IReadOnlyList<Grade>> GetPublishedByStudentAsync(
        Guid studentUserId, CancellationToken ct = default)
    {
        var enrollmentIds = await _context.Enrollments
            .Where(e => e.SchoolId == _schoolId && e.StudentUserId == studentUserId)
            .Select(e => e.EnrollmentId)
            .ToListAsync(ct);

        return await ToListAsync(
            Table.Where(g => g.SchoolId == _schoolId &&
                             g.IsPublished            &&
                             enrollmentIds.Contains(g.EnrollmentId))
                 .OrderBy(g => g.GradedAt), ct);
    }

    public async Task AddAsync(Grade grade, CancellationToken ct = default) =>
        await Table.AddAsync(MapToEntity(grade), ct);

    public void Update(Grade grade) =>
        _context.Update(MapToEntity(grade));

    public async Task PublishAllAsync(Guid courseId, Guid gradePeriodId, CancellationToken ct = default)
    {
        var enrollmentIds = await _context.Enrollments
            .Where(e => e.SchoolId == _schoolId && e.CourseId == courseId)
            .Select(e => e.EnrollmentId)
            .ToListAsync(ct);

        var grades = await Table
            .Where(g => g.SchoolId      == _schoolId    &&
                        g.GradePeriodId  == gradePeriodId &&
                        !g.IsPublished                    &&
                        enrollmentIds.Contains(g.EnrollmentId))
            .ToListAsync(ct);

        var now = DateTime.UtcNow;
        foreach (var g in grades)
        {
            g.IsPublished = true;
            g.PublishedAt = now;
        }
    }

    protected override Grade MapToDomain(GradeEntity e) =>
        Grade.Reconstitute(
            e.GradeId,
            e.EnrollmentId,
            e.GradePeriodId,
            e.SchoolId,
            e.TeacherUserId,
            e.Score,
            Enum.Parse<LetterGrade>(e.LetterGrade),
            e.Comment,
            e.IsPublished,
            e.PublishedAt,
            e.GradedAt);

    protected override GradeEntity MapToEntity(Grade g) => new()
    {
        GradeId       = g.GradeId,
        EnrollmentId  = g.EnrollmentId,
        GradePeriodId = g.GradePeriodId,
        SchoolId      = g.SchoolId,
        TeacherUserId = g.TeacherUserId,
        Score         = g.Score.Value,
        LetterGrade   = g.LetterGrade.ToString(),
        Comment       = g.Comment,
        IsPublished   = g.IsPublished,
        PublishedAt   = g.PublishedAt,
        GradedAt      = g.GradedAt
    };
}
