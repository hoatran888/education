using SchoolSystem.Domain.Models.ValueObjects;

namespace SchoolSystem.Domain.Models.Entities;

public class Schedule
{
    public Guid      ScheduleId      { get; private set; }
    public Guid      CourseId        { get; private set; }
    public Guid      SchoolId        { get; private set; }
    public Guid      RoomId          { get; private set; }
    public Guid      TeacherUserId   { get; private set; }
    public DayOfWeek DayOfWeek       { get; private set; }
    public TimeOnly  StartTime       { get; private set; }
    public TimeOnly  EndTime         { get; private set; }
    public DateRange EffectivePeriod { get; private set; }

    public Course? Course { get; set; }
    public Room?   Room   { get; set; }

    public string? CourseName  { get; set; }
    public string? RoomName    { get; set; }
    public string? TeacherName { get; set; }

    private Schedule() { }

    public static Schedule Create(
        Guid      courseId,
        Guid      schoolId,
        Guid      roomId,
        Guid      teacherUserId,
        DayOfWeek dayOfWeek,
        TimeOnly  startTime,
        TimeOnly  endTime,
        DateRange effectivePeriod)
    {
        if (endTime <= startTime)
            throw new ArgumentException("End time must be after start time.");

        return new Schedule
        {
            ScheduleId      = Guid.NewGuid(),
            CourseId        = courseId,
            SchoolId        = schoolId,
            RoomId          = roomId,
            TeacherUserId   = teacherUserId,
            DayOfWeek       = dayOfWeek,
            StartTime       = startTime,
            EndTime         = endTime,
            EffectivePeriod = effectivePeriod
        };
    }

    public static Schedule Reconstitute(
        Guid      scheduleId,
        Guid      courseId,
        Guid      schoolId,
        Guid      roomId,
        Guid      teacherUserId,
        DayOfWeek dayOfWeek,
        TimeOnly  startTime,
        TimeOnly  endTime,
        DateRange effectivePeriod) => new()
    {
        ScheduleId      = scheduleId,
        CourseId        = courseId,
        SchoolId        = schoolId,
        RoomId          = roomId,
        TeacherUserId   = teacherUserId,
        DayOfWeek       = dayOfWeek,
        StartTime       = startTime,
        EndTime         = endTime,
        EffectivePeriod = effectivePeriod
    };

    public bool ConflictsWith(Schedule other) =>
        RoomId    == other.RoomId    &&
        DayOfWeek == other.DayOfWeek &&
        EffectivePeriod.Overlaps(other.EffectivePeriod) &&
        StartTime < other.EndTime   &&
        EndTime   > other.StartTime;

    public bool TeacherConflictsWith(Schedule other) =>
        TeacherUserId == other.TeacherUserId &&
        DayOfWeek     == other.DayOfWeek     &&
        EffectivePeriod.Overlaps(other.EffectivePeriod) &&
        StartTime < other.EndTime            &&
        EndTime   > other.StartTime;
}
