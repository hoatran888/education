using SchoolSystem.Domain.Models.Entities;

namespace SchoolSystem.Infrastructure.Data.Interfaces;

public interface IGradeRepository
{
    Task<Grade?>              GetByIdAsync(Guid gradeId, CancellationToken ct = default);
    Task<IReadOnlyList<Grade>> GetByEnrollmentAsync(Guid enrollmentId, CancellationToken ct = default);
    Task<IReadOnlyList<Grade>> GetByStudentAsync(Guid studentUserId, CancellationToken ct = default);
    Task<IReadOnlyList<Grade>> GetByCourseAndPeriodAsync(Guid courseId, Guid gradePeriodId, CancellationToken ct = default);
    Task<IReadOnlyList<Grade>> GetPublishedByStudentAsync(Guid studentUserId, CancellationToken ct = default);
    Task                      AddAsync(Grade grade, CancellationToken ct = default);
    void                      Update(Grade grade);
    Task                      PublishAllAsync(Guid courseId, Guid gradePeriodId, CancellationToken ct = default);
}
