using FluentValidation;
using MediatR;
using SchoolSystem.Application.Common.Exceptions;
using SchoolSystem.Application.Common.Interfaces;
using SchoolSystem.Domain.Models.Entities;
using SchoolSystem.Domain.Models.Enums;
using SchoolSystem.Infrastructure.Data.Interfaces;

namespace SchoolSystem.Application.Features.Students.Commands;

[RequireRoles(UserRole.Admin, UserRole.SuperAdmin)]
public record UpdateStudentGradeLevelCommand(Guid StudentProfileId, int GradeLevel) : ICommand;

public class UpdateStudentGradeLevelCommandValidator : AbstractValidator<UpdateStudentGradeLevelCommand>
{
    public UpdateStudentGradeLevelCommandValidator()
    {
        RuleFor(x => x.StudentProfileId).NotEmpty();
        RuleFor(x => x.GradeLevel).InclusiveBetween(1, 13);
    }
}

public class UpdateStudentGradeLevelCommandHandler : IRequestHandler<UpdateStudentGradeLevelCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateStudentGradeLevelCommandHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task Handle(UpdateStudentGradeLevelCommand request, CancellationToken ct)
    {
        var profile = await _unitOfWork.Students.GetByIdAsync(request.StudentProfileId, ct)
            ?? throw new NotFoundException(nameof(StudentProfile), request.StudentProfileId);

        profile.UpdateGradeLevel(request.GradeLevel);
        _unitOfWork.Students.Update(profile);
    }
}
