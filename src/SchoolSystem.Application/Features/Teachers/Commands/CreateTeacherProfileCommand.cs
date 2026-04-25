using FluentValidation;
using MediatR;
using SchoolSystem.Application.Common.Exceptions;
using SchoolSystem.Application.Common.Interfaces;
using SchoolSystem.Domain.Models.Entities;
using SchoolSystem.Domain.Models.Enums;
using SchoolSystem.Infrastructure.Data.Interfaces;
using SchoolSystem.Infrastructure.Services;

namespace SchoolSystem.Application.Features.Teachers.Commands;

[RequireRoles(UserRole.Admin, UserRole.SuperAdmin)]
public record CreateTeacherProfileCommand(
    Guid   UserId,
    string Degree,
    string Specialization) : ICommand<Guid>;

public class CreateTeacherProfileCommandValidator : AbstractValidator<CreateTeacherProfileCommand>
{
    public CreateTeacherProfileCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Degree).NotEmpty().MaximumLength(200);
    }
}

public class CreateTeacherProfileCommandHandler : IRequestHandler<CreateTeacherProfileCommand, Guid>
{
    private readonly IUnitOfWork          _unitOfWork;
    private readonly ICurrentSchoolContext _context;

    public CreateTeacherProfileCommandHandler(IUnitOfWork unitOfWork, ICurrentSchoolContext context)
    {
        _unitOfWork = unitOfWork;
        _context    = context;
    }

    public async Task<Guid> Handle(CreateTeacherProfileCommand request, CancellationToken ct)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(request.UserId, ct)
            ?? throw new NotFoundException(nameof(User), request.UserId);

        var profile = TeacherProfile.Create(user.UserId, _context.SchoolId,
                                            request.Degree, request.Specialization);
        await _unitOfWork.Teachers.AddAsync(profile, ct);
        return profile.TeacherProfileId;
    }
}
