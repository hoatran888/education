using Microsoft.EntityFrameworkCore;
using SchoolSystem.Domain.Models.Entities;
using SchoolSystem.Infrastructure.Data.Context;
using SchoolSystem.Infrastructure.Data.Interfaces;
using SchoolSystem.Infrastructure.Data.LinqEntities;

namespace SchoolSystem.Infrastructure.Data.Repositories;

public class LinqStudentRepository
    : LinqRepositoryBase<StudentProfileEntity, StudentProfile>, IStudentRepository
{
    public LinqStudentRepository(SchoolDataContext context, Guid schoolId)
        : base(context, schoolId) { }

    public async Task<StudentProfile?> GetByIdAsync(Guid studentProfileId, CancellationToken ct = default) =>
        await FirstOrDefaultAsync(
            Table.Where(s => s.SchoolId == _schoolId && s.StudentProfileId == studentProfileId), ct);

    public async Task<StudentProfile?> GetByUserIdAsync(Guid userId, CancellationToken ct = default) =>
        await FirstOrDefaultAsync(
            Table.Where(s => s.SchoolId == _schoolId && s.UserId == userId), ct);

    public async Task<IReadOnlyList<StudentProfile>> GetAllAsync(CancellationToken ct = default) =>
        await ToListAsync(
            Table.Where(s => s.SchoolId == _schoolId && s.IsActive)
                 .OrderBy(s => s.GradeLevel), ct);

    public async Task<IReadOnlyList<StudentProfile>> GetByGradeLevelAsync(
        int gradeLevel, CancellationToken ct = default) =>
        await ToListAsync(
            Table.Where(s => s.SchoolId   == _schoolId  &&
                             s.GradeLevel  == gradeLevel  &&
                             s.IsActive)
                 .OrderBy(s => s.UserId), ct);

    public async Task<User?> GetPrimaryParentAsync(Guid studentUserId, CancellationToken ct = default)
    {
        var parentUserId = await _context.StudentParents
            .Where(sp => sp.SchoolId        == _schoolId     &&
                         sp.StudentUserId   == studentUserId  &&
                         sp.IsPrimaryContact)
            .Select(sp => (Guid?)sp.ParentUserId)
            .FirstOrDefaultAsync(ct);

        if (parentUserId is null) return null;
        return await new LinqUserRepository(_context, _schoolId).GetByIdAsync(parentUserId.Value, ct);
    }

    public async Task AddAsync(StudentProfile profile, CancellationToken ct = default) =>
        await Table.AddAsync(MapToEntity(profile), ct);

    public void Update(StudentProfile profile) =>
        _context.Update(MapToEntity(profile));

    public void Delete(StudentProfile profile) =>
        Table.Remove(MapToEntity(profile));

    protected override StudentProfile MapToDomain(StudentProfileEntity e) =>
        StudentProfile.Reconstitute(
            e.StudentProfileId,
            e.UserId,
            e.SchoolId,
            DateOnly.FromDateTime(e.BirthDate),
            e.GradeLevel,
            e.AdmissionDate,
            e.IsActive);

    protected override StudentProfileEntity MapToEntity(StudentProfile s) => new()
    {
        StudentProfileId = s.StudentProfileId,
        UserId           = s.UserId,
        SchoolId         = s.SchoolId,
        BirthDate        = s.BirthDate.ToDateTime(TimeOnly.MinValue),
        GradeLevel       = s.GradeLevel,
        AdmissionDate    = s.AdmissionDate,
        IsActive         = s.IsActive
    };
}
