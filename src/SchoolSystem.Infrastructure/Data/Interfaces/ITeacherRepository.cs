using SchoolSystem.Domain.Models.Entities;

namespace SchoolSystem.Infrastructure.Data.Interfaces;

public interface ITeacherRepository
{
    Task<TeacherProfile?>              GetByIdAsync(Guid teacherProfileId, CancellationToken ct = default);
    Task<TeacherProfile?>              GetByUserIdAsync(Guid userId, CancellationToken ct = default);
    Task<IReadOnlyList<TeacherProfile>> GetAllAsync(CancellationToken ct = default);
    Task                               AddAsync(TeacherProfile profile, CancellationToken ct = default);
    void                               Update(TeacherProfile profile);
    void                               Delete(TeacherProfile profile);
}
