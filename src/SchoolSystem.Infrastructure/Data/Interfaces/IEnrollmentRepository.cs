using SchoolSystem.Domain.Models.Entities;

namespace SchoolSystem.Infrastructure.Data.Interfaces;

public interface IEnrollmentRepository
{
    Task<Enrollment?>              GetByIdAsync(Guid enrollmentId, CancellationToken ct = default);
    Task<IReadOnlyList<Enrollment>> GetByCourseAsync(Guid courseId, CancellationToken ct = default);
    Task<IReadOnlyList<Enrollment>> GetByStudentAsync(Guid studentUserId, CancellationToken ct = default);
    Task<bool>                     ExistsAsync(Guid courseId, Guid studentUserId, CancellationToken ct = default);
    Task                           AddAsync(Enrollment enrollment, CancellationToken ct = default);
    void                           Update(Enrollment enrollment);
}
