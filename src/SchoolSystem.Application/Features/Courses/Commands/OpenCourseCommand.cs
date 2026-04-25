using FluentValidation;
using MediatR;
using SchoolSystem.Application.Common.Exceptions;
using SchoolSystem.Application.Common.Interfaces;
using SchoolSystem.Domain.Models.Entities;
using SchoolSystem.Domain.Models.Enums;
using SchoolSystem.Infrastructure.Data.Interfaces;

namespace SchoolSystem.Application.Features.Courses.Commands;

[RequireRoles(UserRole.Admin, UserRole.SuperAdmin)]
public record OpenCourseCommand(Guid CourseId) : ICommand;

public class OpenCourseCommandValidator : AbstractValidator<OpenCourseCommand>
{
    public OpenCourseCommandValidator() => RuleFor(x => x.CourseId).NotEmpty();
}

public class OpenCourseCommandHandler : IRequestHandler<OpenCourseCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public OpenCourseCommandHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task Handle(OpenCourseCommand request, CancellationToken ct)
    {
        var course = await _unitOfWork.Courses.GetByIdAsync(request.CourseId, ct)
            ?? throw new NotFoundException(nameof(Course), request.CourseId);

        course.Open();
        _unitOfWork.Courses.Update(course);
    }
}
