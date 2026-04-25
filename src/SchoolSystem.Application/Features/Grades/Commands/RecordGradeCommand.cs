using FluentValidation;
using MediatR;
using SchoolSystem.Application.Common.Exceptions;
using SchoolSystem.Application.Common.Interfaces;
using SchoolSystem.Domain.Models.Entities;
using SchoolSystem.Domain.Models.Enums;
using SchoolSystem.Infrastructure.Data.Interfaces;
using SchoolSystem.Infrastructure.Services;

namespace SchoolSystem.Application.Features.Grades.Commands;

[RequireRoles(UserRole.Teacher, UserRole.Admin)]
public record RecordGradeCommand(
    Guid    EnrollmentId,
    Guid    GradePeriodId,
    decimal Score,
    string? Comment) : ICommand<Guid>;

public class RecordGradeCommandValidator : AbstractValidator<RecordGradeCommand>
{
    public RecordGradeCommandValidator()
    {
        RuleFor(x => x.EnrollmentId).NotEmpty();
        RuleFor(x => x.GradePeriodId).NotEmpty();
        RuleFor(x => x.Score).InclusiveBetween(0, 100);
    }
}

public class RecordGradeCommandHandler : IRequestHandler<RecordGradeCommand, Guid>
{
    private readonly IUnitOfWork          _unitOfWork;
    private readonly ICurrentSchoolContext _context;

    public RecordGradeCommandHandler(IUnitOfWork unitOfWork, ICurrentSchoolContext context)
    {
        _unitOfWork = unitOfWork;
        _context    = context;
    }

    public async Task<Guid> Handle(RecordGradeCommand request, CancellationToken ct)
    {
        var enrollment = await _unitOfWork.Enrollments.GetByIdAsync(request.EnrollmentId, ct)
            ?? throw new NotFoundException(nameof(Enrollment), request.EnrollmentId);

        var period = await _unitOfWork.GradePeriods.GetByIdAsync(request.GradePeriodId, ct)
            ?? throw new NotFoundException(nameof(GradePeriod), request.GradePeriodId);

        var grade = Grade.Create(enrollment, period, _context.UserId, request.Score, request.Comment);
        await _unitOfWork.Grades.AddAsync(grade, ct);
        return grade.GradeId;
    }
}
