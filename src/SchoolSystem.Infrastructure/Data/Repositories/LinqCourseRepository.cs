using System.Reflection;
using Microsoft.EntityFrameworkCore;
using SchoolSystem.Domain.Models.Entities;
using SchoolSystem.Domain.Models.Enums;
using SchoolSystem.Domain.Models.ValueObjects;
using SchoolSystem.Infrastructure.Data.Context;
using SchoolSystem.Infrastructure.Data.Interfaces;
using SchoolSystem.Infrastructure.Data.LinqEntities;

namespace SchoolSystem.Infrastructure.Data.Repositories;

public class LinqCourseRepository
    : LinqRepositoryBase<CourseEntity, Course>, ICourseRepository
{
    public LinqCourseRepository(SchoolDataContext context, Guid schoolId)
        : base(context, schoolId) { }

    public async Task<Course?> GetByIdAsync(Guid courseId, CancellationToken ct = default) =>
        await FirstOrDefaultAsync(
            Table.Where(c => c.SchoolId == _schoolId && c.CourseId == courseId), ct);

    public async Task<Course?> GetByIdWithEnrollmentsAsync(Guid courseId, CancellationToken ct = default)
    {
        var entity = await Table
            .Include(c => c.Enrollments)
            .FirstOrDefaultAsync(c => c.SchoolId == _schoolId && c.CourseId == courseId, ct);

        if (entity is null) return null;

        var course = MapToDomain(entity);
        var enrollments = entity.Enrollments.Select(e => Enrollment.Reconstitute(
            e.EnrollmentId,
            e.CourseId,
            e.StudentUserId,
            e.SchoolId,
            e.EnrolledDate,
            Enum.Parse<EnrollmentStatus>(e.Status),
            e.EnrolledBy,
            e.DroppedDate,
            e.DropReason)).ToList();

        var field = typeof(Course).GetField("_enrollments",
            BindingFlags.NonPublic | BindingFlags.Instance)!;
        ((List<Enrollment>)field.GetValue(course)!).AddRange(enrollments);

        return course;
    }

    public async Task<IReadOnlyList<Course>> GetAllAsync(CancellationToken ct = default) =>
        await ToListAsync(
            Table.Where(c => c.SchoolId == _schoolId)
                 .OrderBy(c => c.GradeLevel).ThenBy(c => c.Name), ct);

    public async Task<IReadOnlyList<Course>> GetByTeacherAsync(
        Guid teacherUserId, CancellationToken ct = default) =>
        await ToListAsync(
            Table.Where(c => c.SchoolId == _schoolId && c.TeacherUserId == teacherUserId)
                 .OrderBy(c => c.Name), ct);

    public async Task<IReadOnlyList<Course>> GetByAcademicYearAsync(
        Guid academicYearId, CancellationToken ct = default) =>
        await ToListAsync(
            Table.Where(c => c.SchoolId == _schoolId && c.AcademicYearId == academicYearId)
                 .OrderBy(c => c.GradeLevel).ThenBy(c => c.Name), ct);

    public async Task<IReadOnlyList<Course>> GetOpenCoursesAsync(CancellationToken ct = default) =>
        await ToListAsync(
            Table.Where(c => c.SchoolId == _schoolId && c.Status == nameof(CourseStatus.Open))
                 .OrderBy(c => c.GradeLevel).ThenBy(c => c.Name), ct);

    public async Task<int> GetEnrollmentCountAsync(Guid courseId, CancellationToken ct = default) =>
        await _context.Enrollments.CountAsync(e =>
            e.SchoolId == _schoolId &&
            e.CourseId == courseId  &&
            e.Status   == nameof(EnrollmentStatus.Active), ct);

    public async Task AddAsync(Course course, CancellationToken ct = default) =>
        await Table.AddAsync(MapToEntity(course), ct);

    public void Update(Course course) =>
        _context.Update(MapToEntity(course));

    public void Delete(Course course) =>
        Table.Remove(MapToEntity(course));

    protected override Course MapToDomain(CourseEntity e) =>
        Course.Reconstitute(
            e.CourseId,
            e.SchoolId,
            e.AcademicYearId,
            e.Name,
            e.Description,
            e.GradeLevel,
            e.Credits,
            e.MaxStudents,
            e.TeacherUserId,
            Enum.Parse<CourseStatus>(e.Status),
            new DateRange(
                DateOnly.FromDateTime(e.StartDate),
                DateOnly.FromDateTime(e.EndDate)),
            e.CreatedAt,
            e.CreatedBy);

    protected override CourseEntity MapToEntity(Course c) => new()
    {
        CourseId       = c.CourseId,
        SchoolId       = c.SchoolId,
        AcademicYearId = c.AcademicYearId,
        Name           = c.Name,
        Description    = c.Description,
        GradeLevel     = c.GradeLevel,
        Credits        = c.Credits,
        MaxStudents    = c.MaxStudents,
        TeacherUserId  = c.TeacherUserId,
        Status         = c.Status.ToString(),
        StartDate      = c.Duration.Start.ToDateTime(TimeOnly.MinValue),
        EndDate        = c.Duration.End.ToDateTime(TimeOnly.MinValue),
        CreatedAt      = c.CreatedAt,
        CreatedBy      = c.CreatedBy
    };
}
