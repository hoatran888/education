using FluentValidation;
using MediatR;
using SchoolSystem.Application.Common.Exceptions;
using SchoolSystem.Application.Common.Interfaces;
using SchoolSystem.Domain.Models.Entities;
using SchoolSystem.Domain.Models.Enums;
using SchoolSystem.Infrastructure.Data.Interfaces;

namespace SchoolSystem.Application.Features.Teachers.Commands;

[RequireRoles(UserRole.Admin, UserRole.SuperAdmin)]
public record UpdateTeacherProfileCommand(
    Guid   TeacherProfileId,
    string Degree,
    string Specialization) : ICommand;

public class UpdateTeacherProfileCommandValidator : AbstractValidator<UpdateTeacherProfileCommand>
{
    public UpdateTeacherProfileCommandValidator()
    {
        RuleFor(x => x.TeacherProfileId).NotEmpty();
        RuleFor(x => x.Degree).NotEmpty().MaximumLength(200);
    }
}

public class UpdateTeacherProfileCommandHandler : IRequestHandler<UpdateTeacherProfileCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateTeacherProfileCommandHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task Handle(UpdateTeacherProfileCommand request, CancellationToken ct)
    {
        var profile = await _unitOfWork.Teachers.GetByIdAsync(request.TeacherProfileId, ct)
            ?? throw new NotFoundException(nameof(TeacherProfile), request.TeacherProfileId);

        profile.Update(request.Degree, request.Specialization);
        _unitOfWork.Teachers.Update(profile);
    }
}
