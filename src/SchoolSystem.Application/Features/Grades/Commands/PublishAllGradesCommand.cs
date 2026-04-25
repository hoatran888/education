using FluentValidation;
using MediatR;
using SchoolSystem.Application.Common.Interfaces;
using SchoolSystem.Domain.Models.Enums;
using SchoolSystem.Infrastructure.Data.Interfaces;

namespace SchoolSystem.Application.Features.Grades.Commands;

[RequireRoles(UserRole.Teacher, UserRole.Admin)]
public record PublishAllGradesCommand(Guid CourseId, Guid GradePeriodId) : ICommand;

public class PublishAllGradesCommandValidator : AbstractValidator<PublishAllGradesCommand>
{
    public PublishAllGradesCommandValidator()
    {
        RuleFor(x => x.CourseId).NotEmpty();
        RuleFor(x => x.GradePeriodId).NotEmpty();
    }
}

public class PublishAllGradesCommandHandler : IRequestHandler<PublishAllGradesCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public PublishAllGradesCommandHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task Handle(PublishAllGradesCommand request, CancellationToken ct)
    {
        await _unitOfWork.Grades.PublishAllAsync(request.CourseId, request.GradePeriodId, ct);
    }
}
