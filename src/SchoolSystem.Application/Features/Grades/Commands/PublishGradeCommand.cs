using FluentValidation;
using MediatR;
using SchoolSystem.Application.Common.Exceptions;
using SchoolSystem.Application.Common.Interfaces;
using SchoolSystem.Domain.Models.Entities;
using SchoolSystem.Domain.Models.Enums;
using SchoolSystem.Domain.Models.Events;
using SchoolSystem.Infrastructure.Data.Interfaces;

namespace SchoolSystem.Application.Features.Grades.Commands;

[RequireRoles(UserRole.Teacher, UserRole.Admin)]
public record PublishGradeCommand(Guid GradeId) : ICommand;

public class PublishGradeCommandValidator : AbstractValidator<PublishGradeCommand>
{
    public PublishGradeCommandValidator() => RuleFor(x => x.GradeId).NotEmpty();
}

public class PublishGradeCommandHandler : IRequestHandler<PublishGradeCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator   _mediator;

    public PublishGradeCommandHandler(IUnitOfWork unitOfWork, IMediator mediator)
    {
        _unitOfWork = unitOfWork;
        _mediator   = mediator;
    }

    public async Task Handle(PublishGradeCommand request, CancellationToken ct)
    {
        var grade = await _unitOfWork.Grades.GetByIdAsync(request.GradeId, ct)
            ?? throw new NotFoundException(nameof(Grade), request.GradeId);

        grade.Publish();
        _unitOfWork.Grades.Update(grade);

        await _mediator.Publish(new GradePublishedDomainEvent(
            grade.GradeId, grade.EnrollmentId, grade.SchoolId), ct);
    }
}
