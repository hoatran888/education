using SchoolSystem.Domain.Models.Entities;

namespace SchoolSystem.Infrastructure.Data.Interfaces;

public interface IStudentRepository
{
    Task<StudentProfile?>              GetByIdAsync(Guid studentProfileId, CancellationToken ct = default);
    Task<StudentProfile?>              GetByUserIdAsync(Guid userId, CancellationToken ct = default);
    Task<IReadOnlyList<StudentProfile>> GetAllAsync(CancellationToken ct = default);
    Task<IReadOnlyList<StudentProfile>> GetByGradeLevelAsync(int gradeLevel, CancellationToken ct = default);
    Task<User?>                        GetPrimaryParentAsync(Guid studentUserId, CancellationToken ct = default);
    Task                               AddAsync(StudentProfile profile, CancellationToken ct = default);
    void                               Update(StudentProfile profile);
    void                               Delete(StudentProfile profile);
}
