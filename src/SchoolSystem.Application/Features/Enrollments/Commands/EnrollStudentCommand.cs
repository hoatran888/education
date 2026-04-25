using FluentValidation;
using MediatR;
using SchoolSystem.Application.Common.Exceptions;
using SchoolSystem.Application.Common.Interfaces;
using SchoolSystem.Domain.Models.Entities;
using SchoolSystem.Domain.Models.Enums;
using SchoolSystem.Domain.Models.Events;
using SchoolSystem.Infrastructure.Data.Interfaces;
using SchoolSystem.Infrastructure.Services;

namespace SchoolSystem.Application.Features.Enrollments.Commands;

[RequireRoles(UserRole.Admin, UserRole.SuperAdmin)]
public record EnrollStudentCommand(Guid CourseId, Guid StudentUserId) : ICommand<Guid>;

public class EnrollStudentCommandValidator : AbstractValidator<EnrollStudentCommand>
{
    public EnrollStudentCommandValidator()
    {
        RuleFor(x => x.CourseId).NotEmpty();
        RuleFor(x => x.StudentUserId).NotEmpty();
    }
}

public class EnrollStudentCommandHandler : IRequestHandler<EnrollStudentCommand, Guid>
{
    private readonly IUnitOfWork          _unitOfWork;
    private readonly ICurrentSchoolContext _context;
    private readonly IMediator            _mediator;

    public EnrollStudentCommandHandler(
        IUnitOfWork unitOfWork, ICurrentSchoolContext context, IMediator mediator)
    {
        _unitOfWork = unitOfWork;
        _context    = context;
        _mediator   = mediator;
    }

    public async Task<Guid> Handle(EnrollStudentCommand request, CancellationToken ct)
    {
        var course = await _unitOfWork.Courses.GetByIdWithEnrollmentsAsync(request.CourseId, ct)
            ?? throw new NotFoundException(nameof(Course), request.CourseId);

        var enrollment = course.EnrollStudent(request.StudentUserId, _context.UserId);
        await _unitOfWork.Enrollments.AddAsync(enrollment, ct);

        await _mediator.Publish(new EnrollmentCreatedDomainEvent(
            enrollment.EnrollmentId, course.CourseId, request.StudentUserId), ct);

        return enrollment.EnrollmentId;
    }
}
