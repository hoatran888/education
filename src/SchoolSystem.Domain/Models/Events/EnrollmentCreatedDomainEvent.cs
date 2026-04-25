using SchoolSystem.Domain.Models.Base;

namespace SchoolSystem.Domain.Models.Events;

public record EnrollmentCreatedDomainEvent(
    Guid EnrollmentId,
    Guid CourseId,
    Guid StudentUserId) : IDomainEvent;
