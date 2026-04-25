using SchoolSystem.Domain.Models.Entities;
using SchoolSystem.Infrastructure.Data.Context;
using SchoolSystem.Infrastructure.Data.Interfaces;
using SchoolSystem.Infrastructure.Data.LinqEntities;

namespace SchoolSystem.Infrastructure.Data.Repositories;

public class LinqTeacherRepository
    : LinqRepositoryBase<TeacherProfileEntity, TeacherProfile>, ITeacherRepository
{
    public LinqTeacherRepository(SchoolDataContext context, Guid schoolId)
        : base(context, schoolId) { }

    public async Task<TeacherProfile?> GetByIdAsync(Guid teacherProfileId, CancellationToken ct = default) =>
        await FirstOrDefaultAsync(
            Table.Where(t => t.SchoolId == _schoolId && t.TeacherProfileId == teacherProfileId), ct);

    public async Task<TeacherProfile?> GetByUserIdAsync(Guid userId, CancellationToken ct = default) =>
        await FirstOrDefaultAsync(
            Table.Where(t => t.SchoolId == _schoolId && t.UserId == userId), ct);

    public async Task<IReadOnlyList<TeacherProfile>> GetAllAsync(CancellationToken ct = default) =>
        await ToListAsync(
            Table.Where(t => t.SchoolId == _schoolId && t.IsActive)
                 .OrderBy(t => t.UserId), ct);

    public async Task AddAsync(TeacherProfile profile, CancellationToken ct = default) =>
        await Table.AddAsync(MapToEntity(profile), ct);

    public void Update(TeacherProfile profile) =>
        _context.Update(MapToEntity(profile));

    public void Delete(TeacherProfile profile) =>
        Table.Remove(MapToEntity(profile));

    protected override TeacherProfile MapToDomain(TeacherProfileEntity e) =>
        TeacherProfile.Reconstitute(
            e.TeacherProfileId,
            e.UserId,
            e.SchoolId,
            e.Degree,
            e.Specialization,
            e.HireDate,
            e.IsActive);

    protected override TeacherProfileEntity MapToEntity(TeacherProfile t) => new()
    {
        TeacherProfileId = t.TeacherProfileId,
        UserId           = t.UserId,
        SchoolId         = t.SchoolId,
        Degree           = t.Degree,
        Specialization   = t.Specialization,
        HireDate         = t.HireDate,
        IsActive         = t.IsActive
    };
}
