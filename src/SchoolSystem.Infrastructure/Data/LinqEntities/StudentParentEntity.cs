namespace SchoolSystem.Infrastructure.Data.LinqEntities;

public class StudentParentEntity
{
    public Guid StudentParentId  { get; set; }
    public Guid StudentUserId    { get; set; }
    public Guid ParentUserId     { get; set; }
    public Guid SchoolId         { get; set; }
    public bool IsPrimaryContact { get; set; }
    public int  Relationship     { get; set; }
}
