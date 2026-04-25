using FluentValidation;
using MediatR;
using SchoolSystem.Application.Common.Interfaces;
using SchoolSystem.Domain.Models.Entities;
using SchoolSystem.Domain.Models.Enums;
using SchoolSystem.Domain.Models.ValueObjects;
using SchoolSystem.Infrastructure.Data.Interfaces;
using SchoolSystem.Infrastructure.Services;

namespace SchoolSystem.Application.Features.Courses.Commands;

[RequireRoles(UserRole.Admin, UserRole.SuperAdmin)]
public record CreateCourseCommand(
    Guid     AcademicYearId,
    string   Name,
    string?  Description,
    int      GradeLevel,
    int      Credits,
    int      MaxStudents,
    DateOnly DurationStart,
    DateOnly DurationEnd) : ICommand<Guid>;

public class CreateCourseCommandValidator : AbstractValidator<CreateCourseCommand>
{
    public CreateCourseCommandValidator()
    {
        RuleFor(x => x.AcademicYearId).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.GradeLevel).InclusiveBetween(1, 13);
        RuleFor(x => x.Credits).GreaterThan(0);
        RuleFor(x => x.MaxStudents).GreaterThan(0);
        RuleFor(x => x.DurationEnd).GreaterThan(x => x.DurationStart);
    }
}

public class CreateCourseCommandHandler : IRequestHandler<CreateCourseCommand, Guid>
{
    private readonly IUnitOfWork          _unitOfWork;
    private readonly ICurrentSchoolContext _context;

    public CreateCourseCommandHandler(IUnitOfWork unitOfWork, ICurrentSchoolContext context)
    {
        _unitOfWork = unitOfWork;
        _context    = context;
    }

    public async Task<Guid> Handle(CreateCourseCommand request, CancellationToken ct)
    {
        var duration = new DateRange(request.DurationStart, request.DurationEnd);
        var course   = Course.Create(_context.SchoolId, request.AcademicYearId,
                                     request.Name, request.Description,
                                     request.GradeLevel, request.Credits,
                                     request.MaxStudents, duration, _context.UserId);
        await _unitOfWork.Courses.AddAsync(course, ct);
        return course.CourseId;
    }
}
