namespace SchoolSystem.Application.Features.Schedules.DTOs;

public record ScheduleDto(
    Guid      ScheduleId,
    Guid      CourseId,
    Guid      SchoolId,
    Guid      RoomId,
    Guid      TeacherUserId,
    string    DayOfWeek,
    TimeOnly  StartTime,
    TimeOnly  EndTime,
    DateOnly  EffectiveFrom,
    DateOnly  EffectiveTo,
    string?   CourseName,
    string?   RoomName,
    string?   TeacherName);
