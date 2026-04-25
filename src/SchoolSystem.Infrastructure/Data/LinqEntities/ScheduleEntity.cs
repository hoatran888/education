namespace SchoolSystem.Infrastructure.Data.LinqEntities;

public class ScheduleEntity
{
    public Guid     ScheduleId    { get; set; }
    public Guid     CourseId      { get; set; }
    public Guid     SchoolId      { get; set; }
    public Guid     RoomId        { get; set; }
    public Guid     TeacherUserId { get; set; }
    public int      DayOfWeek     { get; set; }
    public TimeSpan StartTime     { get; set; }
    public TimeSpan EndTime       { get; set; }
    public DateTime EffectiveFrom { get; set; }
    public DateTime EffectiveTo   { get; set; }
}
