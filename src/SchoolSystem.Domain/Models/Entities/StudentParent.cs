using SchoolSystem.Domain.Models.Enums;

namespace SchoolSystem.Domain.Models.Entities;

public class StudentParent
{
    public Guid               StudentParentId  { get; private set; }
    public Guid               StudentUserId    { get; private set; }
    public Guid               ParentUserId     { get; private set; }
    public Guid               SchoolId         { get; private set; }
    public bool               IsPrimaryContact { get; private set; }
    public ParentRelationship Relationship     { get; private set; }

    public User? Student { get; set; }
    public User? Parent  { get; set; }

    private StudentParent() { }

    public static StudentParent Create(
        Guid              studentUserId,
        Guid              parentUserId,
        Guid              schoolId,
        ParentRelationship relationship,
        bool              isPrimaryContact = false)
    {
        return new StudentParent
        {
            StudentParentId  = Guid.NewGuid(),
            StudentUserId    = studentUserId,
            ParentUserId     = parentUserId,
            SchoolId         = schoolId,
            Relationship     = relationship,
            IsPrimaryContact = isPrimaryContact
        };
    }

    public void SetAsPrimary()   => IsPrimaryContact = true;
    public void UnsetAsPrimary() => IsPrimaryContact = false;
}
