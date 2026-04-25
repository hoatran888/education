using SchoolSystem.Domain.Models.Enums;

namespace SchoolSystem.Domain.Models.Entities;

public class ParentProfile
{
    private readonly List<StudentParent> _children = new();

    public Guid               ParentProfileId { get; private set; }
    public Guid               UserId          { get; private set; }
    public Guid               SchoolId        { get; private set; }
    public ParentRelationship Relationship    { get; private set; }

    public User? User { get; set; }
    public IReadOnlyList<StudentParent> Children => _children.AsReadOnly();

    private ParentProfile() { }

    public static ParentProfile Create(
        Guid              userId,
        Guid              schoolId,
        ParentRelationship relationship)
    {
        return new ParentProfile
        {
            ParentProfileId = Guid.NewGuid(),
            UserId          = userId,
            SchoolId        = schoolId,
            Relationship    = relationship
        };
    }

    public void UpdateRelationship(ParentRelationship relationship) =>
        Relationship = relationship;

    public void AddChild(StudentParent link) => _children.Add(link);
}
