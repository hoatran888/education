using FluentValidation;
using MediatR;
using SchoolSystem.Application.Common.Exceptions;
using SchoolSystem.Application.Common.Interfaces;
using SchoolSystem.Domain.Models.Entities;
using SchoolSystem.Domain.Models.Enums;
using SchoolSystem.Infrastructure.Data.Interfaces;
using SchoolSystem.Infrastructure.Services;

namespace SchoolSystem.Application.Features.Students.Commands;

[RequireRoles(UserRole.Admin, UserRole.SuperAdmin)]
public record CreateStudentProfileCommand(
    Guid     UserId,
    DateOnly BirthDate,
    int      GradeLevel) : ICommand<Guid>;

public class CreateStudentProfileCommandValidator : AbstractValidator<CreateStudentProfileCommand>
{
    public CreateStudentProfileCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.GradeLevel).InclusiveBetween(1, 13);
        RuleFor(x => x.BirthDate).LessThan(DateOnly.FromDateTime(DateTime.UtcNow));
    }
}

public class CreateStudentProfileCommandHandler : IRequestHandler<CreateStudentProfileCommand, Guid>
{
    private readonly IUnitOfWork          _unitOfWork;
    private readonly ICurrentSchoolContext _context;

    public CreateStudentProfileCommandHandler(IUnitOfWork unitOfWork, ICurrentSchoolContext context)
    {
        _unitOfWork = unitOfWork;
        _context    = context;
    }

    public async Task<Guid> Handle(CreateStudentProfileCommand request, CancellationToken ct)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(request.UserId, ct)
            ?? throw new NotFoundException(nameof(User), request.UserId);

        var profile = StudentProfile.Create(user.UserId, _context.SchoolId,
                                            request.BirthDate, request.GradeLevel);
        await _unitOfWork.Students.AddAsync(profile, ct);
        return profile.StudentProfileId;
    }
}
