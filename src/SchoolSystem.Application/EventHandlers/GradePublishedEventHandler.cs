using MediatR;
using Microsoft.Extensions.Logging;
using SchoolSystem.Domain.Models.Events;

namespace SchoolSystem.Application.EventHandlers;

public class GradePublishedEventHandler : INotificationHandler<GradePublishedDomainEvent>
{
    private readonly ILogger<GradePublishedEventHandler> _logger;

    public GradePublishedEventHandler(ILogger<GradePublishedEventHandler> logger)
        => _logger = logger;

    public Task Handle(GradePublishedDomainEvent notification, CancellationToken ct)
    {
        _logger.LogInformation(
            "Grade {GradeId} published for enrollment {EnrollmentId} in school {SchoolId}.",
            notification.GradeId, notification.EnrollmentId, notification.SchoolId);

        return Task.CompletedTask;
    }
}
