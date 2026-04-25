using SchoolSystem.Domain.Models.Entities;
using SchoolSystem.Domain.Models.Enums;
using SchoolSystem.Infrastructure.Data.Context;
using SchoolSystem.Infrastructure.Data.Interfaces;
using SchoolSystem.Infrastructure.Data.LinqEntities;

namespace SchoolSystem.Infrastructure.Data.Repositories;

public class LinqEnrollmentRepository
    : LinqRepositoryBase<EnrollmentEntity, Enrollment>, IEnrollmentRepository
{
    public LinqEnrollmentRepository(SchoolDataContext context, Guid schoolId)
        : base(context, schoolId) { }

    public async Task<Enrollment?> GetByIdAsync(Guid enrollmentId, CancellationToken ct = default) =>
        await FirstOrDefaultAsync(
            Table.Where(e => e.SchoolId == _schoolId && e.EnrollmentId == enrollmentId), ct);

    public async Task<IReadOnlyList<Enrollment>> GetByCourseAsync(
        Guid courseId, CancellationToken ct = default) =>
        await ToListAsync(
            Table.Where(e => e.SchoolId == _schoolId && e.CourseId == courseId)
                 .OrderBy(e => e.EnrolledDate), ct);

    public async Task<IReadOnlyList<Enrollment>> GetByStudentAsync(
        Guid studentUserId, CancellationToken ct = default) =>
        await ToListAsync(
            Table.Where(e => e.SchoolId == _schoolId && e.StudentUserId == studentUserId)
                 .OrderBy(e => e.EnrolledDate), ct);

    public async Task<bool> ExistsAsync(
        Guid courseId, Guid studentUserId, CancellationToken ct = default) =>
        await AnyAsync(
            Table.Where(e => e.SchoolId     == _schoolId    &&
                             e.CourseId      == courseId     &&
                             e.StudentUserId == studentUserId &&
                             e.Status        != nameof(EnrollmentStatus.Dropped)), ct);

    public async Task AddAsync(Enrollment enrollment, CancellationToken ct = default) =>
        await Table.AddAsync(MapToEntity(enrollment), ct);

    public void Update(Enrollment enrollment) =>
        _context.Update(MapToEntity(enrollment));

    protected override Enrollment MapToDomain(EnrollmentEntity e) =>
        Enrollment.Reconstitute(
            e.EnrollmentId,
            e.CourseId,
            e.StudentUserId,
            e.SchoolId,
            e.EnrolledDate,
            Enum.Parse<EnrollmentStatus>(e.Status),
            e.EnrolledBy,
            e.DroppedDate,
            e.DropReason);

    protected override EnrollmentEntity MapToEntity(Enrollment e) => new()
    {
        EnrollmentId  = e.EnrollmentId,
        CourseId      = e.CourseId,
        StudentUserId = e.StudentUserId,
        SchoolId      = e.SchoolId,
        EnrolledDate  = e.EnrolledDate,
        Status        = e.Status.ToString(),
        EnrolledBy    = e.EnrolledBy,
        DroppedDate   = e.DroppedDate,
        DropReason    = e.DropReason
    };
}
