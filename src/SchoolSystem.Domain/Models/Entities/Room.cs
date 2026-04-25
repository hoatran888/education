using SchoolSystem.Domain.Models.Enums;

namespace SchoolSystem.Domain.Models.Entities;

public class Room
{
    public Guid     RoomId   { get; private set; }
    public Guid     SchoolId { get; private set; }
    public string   Name     { get; private set; }
    public string?  Building { get; private set; }
    public int      Capacity { get; private set; }
    public RoomType RoomType { get; private set; }
    public bool     IsActive { get; private set; }

    private Room() { }

    public static Room Create(
        Guid     schoolId,
        string   name,
        int      capacity,
        RoomType roomType,
        string?  building = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Room name is required.");
        if (capacity < 1)
            throw new ArgumentException("Capacity must be at least 1.");

        return new Room
        {
            RoomId   = Guid.NewGuid(),
            SchoolId = schoolId,
            Name     = name.Trim(),
            Building = building?.Trim(),
            Capacity = capacity,
            RoomType = roomType,
            IsActive = true
        };
    }

    public void Update(string name, int capacity, RoomType roomType, string? building)
    {
        Name     = name.Trim();
        Capacity = capacity;
        RoomType = roomType;
        Building = building?.Trim();
    }

    public void Deactivate() => IsActive = false;
    public void Activate()   => IsActive = true;
}
