namespace SchoolSystem.Infrastructure.Data.LinqEntities;

public class ParentProfileEntity
{
    public Guid ParentProfileId { get; set; }
    public Guid UserId          { get; set; }
    public Guid SchoolId        { get; set; }
    public int  Relationship    { get; set; }
}
