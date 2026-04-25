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
public record UpdateGradeCommand(Guid GradeId, decimal Score, string? Comment) : ICommand;

public class UpdateGradeCommandValidator : AbstractValidator<UpdateGradeCommand>
{
    public UpdateGradeCommandValidator()
    {
        RuleFor(x => x.GradeId).NotEmpty();
        RuleFor(x => x.Score).InclusiveBetween(0, 100);
    }
}

public class UpdateGradeCommandHandler : IRequestHandler<UpdateGradeCommand>
{
    private readonly IUnitOfWork          _unitOfWork;
    private readonly ICurrentSchoolContext _context;

    public UpdateGradeCommandHandler(IUnitOfWork unitOfWork, ICurrentSchoolContext context)
    {
        _unitOfWork = unitOfWork;
        _context    = context;
    }

    public async Task Handle(UpdateGradeCommand request, CancellationToken ct)
    {
        var grade = await _unitOfWork.Grades.GetByIdAsync(request.GradeId, ct)
            ?? throw new NotFoundException(nameof(Grade), request.GradeId);

        if (grade.TeacherUserId != _context.UserId)
            throw new ForbiddenException("Only the grading teacher can update this grade.");

        grade.UpdateScore(request.Score, request.Comment);
        _unitOfWork.Grades.Update(grade);
    }
}
