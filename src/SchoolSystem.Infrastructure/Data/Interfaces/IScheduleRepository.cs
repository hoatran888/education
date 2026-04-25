using SchoolSystem.Domain.Models.Entities;

namespace SchoolSystem.Infrastructure.Data.Interfaces;

public interface IScheduleRepository
{
    Task<Schedule?>              GetByIdAsync(Guid scheduleId, CancellationToken ct = default);
    Task<IReadOnlyList<Schedule>> GetByCourseAsync(Guid courseId, CancellationToken ct = default);
    Task<IReadOnlyList<Schedule>> GetByRoomAsync(Guid roomId, DateTime date, CancellationToken ct = default);
    Task<IReadOnlyList<Schedule>> GetByTeacherAsync(Guid teacherUserId, CancellationToken ct = default);
    Task<IReadOnlyList<Schedule>> GetByMonthAsync(int month, int year, CancellationToken ct = default);
    Task<bool>                   HasRoomConflictAsync(Guid roomId, int dayOfWeek, TimeSpan startTime, TimeSpan endTime, DateTime effectiveFrom, DateTime effectiveTo, CancellationToken ct = default);
    Task<bool>                   HasTeacherConflictAsync(Guid teacherUserId, int dayOfWeek, TimeSpan startTime, TimeSpan endTime, DateTime effectiveFrom, DateTime effectiveTo, CancellationToken ct = default);
    Task                         AddAsync(Schedule schedule, CancellationToken ct = default);
    void                         Update(Schedule schedule);
    void                         Delete(Schedule schedule);
}
