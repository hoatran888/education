using SchoolSystem.Domain.Models.Base;

namespace SchoolSystem.Domain.Models.Events;

public record GradePublishedDomainEvent(
    Guid GradeId,
    Guid EnrollmentId,
    Guid SchoolId) : IDomainEvent;
