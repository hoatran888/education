using FluentValidation;
using MediatR;
using SchoolSystem.Application.Common.Exceptions;
using SchoolSystem.Application.Common.Interfaces;
using SchoolSystem.Domain.Models.Entities;
using SchoolSystem.Domain.Models.Enums;
using SchoolSystem.Infrastructure.Data.Interfaces;

namespace SchoolSystem.Application.Features.Schedules.Commands;

[RequireRoles(UserRole.Admin, UserRole.SuperAdmin)]
public record DeleteScheduleCommand(Guid ScheduleId) : ICommand;

public class DeleteScheduleCommandValidator : AbstractValidator<DeleteScheduleCommand>
{
    public DeleteScheduleCommandValidator() => RuleFor(x => x.ScheduleId).NotEmpty();
}

public class DeleteScheduleCommandHandler : IRequestHandler<DeleteScheduleCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteScheduleCommandHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task Handle(DeleteScheduleCommand request, CancellationToken ct)
    {
        var schedule = await _unitOfWork.Schedules.GetByIdAsync(request.ScheduleId, ct)
            ?? throw new NotFoundException(nameof(Schedule), request.ScheduleId);

        _unitOfWork.Schedules.Delete(schedule);
    }
}
