namespace SchoolSystem.Infrastructure.Data.LinqEntities;

public class RoomEntity
{
    public Guid    RoomId    { get; set; }
    public Guid    SchoolId  { get; set; }
    public string  Name      { get; set; } = string.Empty;
    public string? Building  { get; set; }
    public int     Capacity  { get; set; }
    public int     RoomType  { get; set; }
    public bool    IsActive  { get; set; }
}
