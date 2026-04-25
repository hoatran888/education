using SchoolSystem.Domain.Models.Entities;

namespace SchoolSystem.Infrastructure.Data.Interfaces;

public interface ISchoolRepository
{
    Task<School?>              GetByIdAsync(Guid schoolId, CancellationToken ct = default);
    Task<IReadOnlyList<School>> GetAllAsync(CancellationToken ct = default);
    Task                       AddAsync(School school, CancellationToken ct = default);
    void                       Update(School school);
    void                       Delete(School school);
}
