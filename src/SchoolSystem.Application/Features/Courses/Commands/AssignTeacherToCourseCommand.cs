using FluentValidation;
using MediatR;
using SchoolSystem.Application.Common.Exceptions;
using SchoolSystem.Application.Common.Interfaces;
using SchoolSystem.Domain.Models.Entities;
using SchoolSystem.Domain.Models.Enums;
using SchoolSystem.Infrastructure.Data.Interfaces;

namespace SchoolSystem.Application.Features.Courses.Commands;

[RequireRoles(UserRole.Admin, UserRole.SuperAdmin)]
public record AssignTeacherToCourseCommand(Guid CourseId, Guid TeacherUserId) : ICommand;

public class AssignTeacherToCourseCommandValidator : AbstractValidator<AssignTeacherToCourseCommand>
{
    public AssignTeacherToCourseCommandValidator()
    {
        RuleFor(x => x.CourseId).NotEmpty();
        RuleFor(x => x.TeacherUserId).NotEmpty();
    }
}

public class AssignTeacherToCourseCommandHandler : IRequestHandler<AssignTeacherToCourseCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public AssignTeacherToCourseCommandHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task Handle(AssignTeacherToCourseCommand request, CancellationToken ct)
    {
        var course = await _unitOfWork.Courses.GetByIdAsync(request.CourseId, ct)
            ?? throw new NotFoundException(nameof(Course), request.CourseId);

        _ = await _unitOfWork.Users.GetByIdAsync(request.TeacherUserId, ct)
            ?? throw new NotFoundException(nameof(User), request.TeacherUserId);

        course.AssignTeacher(request.TeacherUserId);
        _unitOfWork.Courses.Update(course);
    }
}
