namespace SchoolSystem.Infrastructure.Data.LinqEntities;

public class UserEntity
{
    public Guid    UserId      { get; set; }
    public Guid    SchoolId    { get; set; }
    public string  B2CObjectId { get; set; } = string.Empty;
    public string  FirstName   { get; set; } = string.Empty;
    public string  LastName    { get; set; } = string.Empty;
    public int     Sex         { get; set; }
    public string  Email       { get; set; } = string.Empty;
    public string  Phone       { get; set; } = string.Empty;
    public string  Street      { get; set; } = string.Empty;
    public string  City        { get; set; } = string.Empty;
    public string  State       { get; set; } = string.Empty;
    public string  ZipCode     { get; set; } = string.Empty;
    public string  Country     { get; set; } = string.Empty;
    public string? PhotoUrl    { get; set; }
    public bool    IsActive    { get; set; }
    public DateTime CreatedAt  { get; set; }

    public string FullName => $"{FirstName} {LastName}";

    public ICollection<UserRoleEntity> UserRoles { get; set; } = [];
}
