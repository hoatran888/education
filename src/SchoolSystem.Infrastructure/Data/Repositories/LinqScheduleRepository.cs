using Microsoft.EntityFrameworkCore;
using SchoolSystem.Domain.Models.Entities;
using SchoolSystem.Domain.Models.ValueObjects;
using SchoolSystem.Infrastructure.Data.Context;
using SchoolSystem.Infrastructure.Data.Interfaces;
using SchoolSystem.Infrastructure.Data.LinqEntities;

namespace SchoolSystem.Infrastructure.Data.Repositories;

public class LinqScheduleRepository
    : LinqRepositoryBase<ScheduleEntity, Schedule>, IScheduleRepository
{
    public LinqScheduleRepository(SchoolDataContext context, Guid schoolId)
        : base(context, schoolId) { }

    public async Task<Schedule?> GetByIdAsync(Guid scheduleId, CancellationToken ct = default) =>
        await FirstOrDefaultAsync(
            Table.Where(s => s.SchoolId == _schoolId && s.ScheduleId == scheduleId), ct);

    public async Task<IReadOnlyList<Schedule>> GetByCourseAsync(
        Guid courseId, CancellationToken ct = default)
    {
        var rows = await Table
            .Where(s => s.SchoolId == _schoolId && s.CourseId == courseId)
            .OrderBy(s => s.DayOfWeek)
            .ToListAsync(ct);
        return rows.OrderBy(s => s.DayOfWeek).ThenBy(s => s.StartTime)
                   .Select(MapToDomain).ToList();
    }

    public async Task<IReadOnlyList<Schedule>> GetByRoomAsync(
        Guid roomId, DateTime date, CancellationToken ct = default)
    {
        var rows = await Table
            .Where(s => s.SchoolId     == _schoolId &&
                        s.RoomId        == roomId    &&
                        s.EffectiveFrom <= date       &&
                        s.EffectiveTo   >= date)
            .OrderBy(s => s.DayOfWeek)
            .ToListAsync(ct);
        return rows.OrderBy(s => s.DayOfWeek).ThenBy(s => s.StartTime)
                   .Select(MapToDomain).ToList();
    }

    public async Task<IReadOnlyList<Schedule>> GetByTeacherAsync(
        Guid teacherUserId, CancellationToken ct = default)
    {
        var rows = await Table
            .Where(s => s.SchoolId == _schoolId && s.TeacherUserId == teacherUserId)
            .OrderBy(s => s.DayOfWeek)
            .ToListAsync(ct);
        return rows.OrderBy(s => s.DayOfWeek).ThenBy(s => s.StartTime)
                   .Select(MapToDomain).ToList();
    }

    public async Task<IReadOnlyList<Schedule>> GetByMonthAsync(
        int month, int year, CancellationToken ct = default)
    {
        var monthStart = new DateTime(year, month, 1);
        var monthEnd   = monthStart.AddMonths(1).AddDays(-1);
        var rows = await Table
            .Where(s => s.SchoolId     == _schoolId &&
                        s.EffectiveFrom <= monthEnd   &&
                        s.EffectiveTo   >= monthStart)
            .OrderBy(s => s.DayOfWeek)
            .ToListAsync(ct);
        return rows.OrderBy(s => s.DayOfWeek).ThenBy(s => s.StartTime)
                   .Select(MapToDomain).ToList();
    }

    public async Task<bool> HasRoomConflictAsync(
        Guid roomId, int dayOfWeek, TimeSpan startTime, TimeSpan endTime,
        DateTime effectiveFrom, DateTime effectiveTo, CancellationToken ct = default) =>
        await AnyAsync(
            Table.Where(s => s.SchoolId     == _schoolId  &&
                             s.RoomId        == roomId      &&
                             s.DayOfWeek     == dayOfWeek   &&
                             s.EffectiveFrom <= effectiveTo  &&
                             s.EffectiveTo   >= effectiveFrom &&
                             s.StartTime     <  endTime      &&
                             s.EndTime       >  startTime), ct);

    public async Task<bool> HasTeacherConflictAsync(
        Guid teacherUserId, int dayOfWeek, TimeSpan startTime, TimeSpan endTime,
        DateTime effectiveFrom, DateTime effectiveTo, CancellationToken ct = default) =>
        await AnyAsync(
            Table.Where(s => s.SchoolId     == _schoolId     &&
                             s.TeacherUserId == teacherUserId  &&
                             s.DayOfWeek     == dayOfWeek       &&
                             s.EffectiveFrom <= effectiveTo      &&
                             s.EffectiveTo   >= effectiveFrom    &&
                             s.StartTime     <  endTime          &&
                             s.EndTime       >  startTime), ct);

    public async Task AddAsync(Schedule schedule, CancellationToken ct = default) =>
        await Table.AddAsync(MapToEntity(schedule), ct);

    public void Update(Schedule schedule) =>
        _context.Update(MapToEntity(schedule));

    public void Delete(Schedule schedule) =>
        Table.Remove(MapToEntity(schedule));

    protected override Schedule MapToDomain(ScheduleEntity e) =>
        Schedule.Reconstitute(
            e.ScheduleId,
            e.CourseId,
            e.SchoolId,
            e.RoomId,
            e.TeacherUserId,
            (DayOfWeek)e.DayOfWeek,
            TimeOnly.FromTimeSpan(e.StartTime),
            TimeOnly.FromTimeSpan(e.EndTime),
            new DateRange(
                DateOnly.FromDateTime(e.EffectiveFrom),
                DateOnly.FromDateTime(e.EffectiveTo)));

    protected override ScheduleEntity MapToEntity(Schedule s) => new()
    {
        ScheduleId    = s.ScheduleId,
        CourseId      = s.CourseId,
        SchoolId      = s.SchoolId,
        RoomId        = s.RoomId,
        TeacherUserId = s.TeacherUserId,
        DayOfWeek     = (int)s.DayOfWeek,
        StartTime     = s.StartTime.ToTimeSpan(),
        EndTime       = s.EndTime.ToTimeSpan(),
        EffectiveFrom = s.EffectivePeriod.Start.ToDateTime(TimeOnly.MinValue),
        EffectiveTo   = s.EffectivePeriod.End.ToDateTime(TimeOnly.MinValue)
    };
}
