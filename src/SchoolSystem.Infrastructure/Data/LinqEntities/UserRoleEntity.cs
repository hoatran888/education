namespace SchoolSystem.Infrastructure.Data.LinqEntities;

public class UserRoleEntity
{
    public Guid UserRoleId { get; set; }
    public Guid UserId     { get; set; }
    public Guid SchoolId   { get; set; }
    public int  Role       { get; set; }
}
