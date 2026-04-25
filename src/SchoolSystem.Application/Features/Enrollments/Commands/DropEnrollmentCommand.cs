using FluentValidation;
using MediatR;
using SchoolSystem.Application.Common.Exceptions;
using SchoolSystem.Application.Common.Interfaces;
using SchoolSystem.Domain.Models.Entities;
using SchoolSystem.Domain.Models.Enums;
using SchoolSystem.Infrastructure.Data.Interfaces;

namespace SchoolSystem.Application.Features.Enrollments.Commands;

[RequireRoles(UserRole.Admin, UserRole.SuperAdmin)]
public record DropEnrollmentCommand(Guid EnrollmentId, string Reason) : ICommand;

public class DropEnrollmentCommandValidator : AbstractValidator<DropEnrollmentCommand>
{
    public DropEnrollmentCommandValidator()
    {
        RuleFor(x => x.EnrollmentId).NotEmpty();
        RuleFor(x => x.Reason).NotEmpty().MaximumLength(500);
    }
}

public class DropEnrollmentCommandHandler : IRequestHandler<DropEnrollmentCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public DropEnrollmentCommandHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task Handle(DropEnrollmentCommand request, CancellationToken ct)
    {
        var enrollment = await _unitOfWork.Enrollments.GetByIdAsync(request.EnrollmentId, ct)
            ?? throw new NotFoundException(nameof(Enrollment), request.EnrollmentId);

        enrollment.Drop(request.Reason);
        _unitOfWork.Enrollments.Update(enrollment);
    }
}
