namespace SchoolSystem.Infrastructure.Data.LinqEntities;

public class HRProfileEntity
{
    public Guid     HRProfileId { get; set; }
    public Guid     UserId      { get; set; }
    public Guid     SchoolId    { get; set; }
    public int      HRType      { get; set; }
    public string?  Department  { get; set; }
    public DateTime HireDate    { get; set; }
    public bool     IsActive    { get; set; }
}
