using FluentValidation;
using MediatR;
using SchoolSystem.Application.Common.Exceptions;
using SchoolSystem.Application.Common.Interfaces;
using SchoolSystem.Domain.Models.Entities;
using SchoolSystem.Domain.Models.Enums;
using SchoolSystem.Infrastructure.Data.Interfaces;

namespace SchoolSystem.Application.Features.Courses.Commands;

[RequireRoles(UserRole.Admin, UserRole.SuperAdmin)]
public record CloseCourseCommand(Guid CourseId) : ICommand;

public class CloseCourseCommandValidator : AbstractValidator<CloseCourseCommand>
{
    public CloseCourseCommandValidator() => RuleFor(x => x.CourseId).NotEmpty();
}

public class CloseCourseCommandHandler : IRequestHandler<CloseCourseCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public CloseCourseCommandHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task Handle(CloseCourseCommand request, CancellationToken ct)
    {
        var course = await _unitOfWork.Courses.GetByIdAsync(request.CourseId, ct)
            ?? throw new NotFoundException(nameof(Course), request.CourseId);

        course.Close();
        _unitOfWork.Courses.Update(course);
    }
}
