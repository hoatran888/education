using MediatR;
using Microsoft.Extensions.Logging;
using SchoolSystem.Domain.Models.Events;

namespace SchoolSystem.Application.EventHandlers;

public class EnrollmentCreatedEventHandler : INotificationHandler<EnrollmentCreatedDomainEvent>
{
    private readonly ILogger<EnrollmentCreatedEventHandler> _logger;

    public EnrollmentCreatedEventHandler(ILogger<EnrollmentCreatedEventHandler> logger)
        => _logger = logger;

    public Task Handle(EnrollmentCreatedDomainEvent notification, CancellationToken ct)
    {
        _logger.LogInformation(
            "Enrollment {EnrollmentId} created for student {StudentUserId} in course {CourseId}.",
            notification.EnrollmentId, notification.StudentUserId, notification.CourseId);

        return Task.CompletedTask;
    }
}
