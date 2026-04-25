using SchoolSystem.Domain.Models.Entities;

namespace SchoolSystem.Infrastructure.Data.Interfaces;

public interface ICourseRepository
{
    Task<Course?>              GetByIdAsync(Guid courseId, CancellationToken ct = default);
    Task<Course?>              GetByIdWithEnrollmentsAsync(Guid courseId, CancellationToken ct = default);
    Task<IReadOnlyList<Course>> GetAllAsync(CancellationToken ct = default);
    Task<IReadOnlyList<Course>> GetByTeacherAsync(Guid teacherUserId, CancellationToken ct = default);
    Task<IReadOnlyList<Course>> GetByAcademicYearAsync(Guid academicYearId, CancellationToken ct = default);
    Task<IReadOnlyList<Course>> GetOpenCoursesAsync(CancellationToken ct = default);
    Task<int>                  GetEnrollmentCountAsync(Guid courseId, CancellationToken ct = default);
    Task                       AddAsync(Course course, CancellationToken ct = default);
    void                       Update(Course course);
    void                       Delete(Course course);
}
