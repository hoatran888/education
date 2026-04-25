namespace SchoolSystem.Infrastructure.Data.LinqEntities;

public class SchoolEntity
{
    public Guid    SchoolId  { get; set; }
    public string  Name      { get; set; } = string.Empty;
    public string  Street    { get; set; } = string.Empty;
    public string  City      { get; set; } = string.Empty;
    public string  State     { get; set; } = string.Empty;
    public string  ZipCode   { get; set; } = string.Empty;
    public string  Country   { get; set; } = string.Empty;
    public string  Email     { get; set; } = string.Empty;
    public string  Phone     { get; set; } = string.Empty;
    public string? LogoUrl   { get; set; }
    public bool    IsActive  { get; set; }
    public DateTime CreatedAt { get; set; }

    public ICollection<UserEntity>   Users   { get; set; } = [];
    public ICollection<CourseEntity> Courses { get; set; } = [];
}
