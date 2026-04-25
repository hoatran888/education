using FluentValidation;
using MediatR;
using SchoolSystem.Application.Common.Exceptions;
using SchoolSystem.Application.Common.Interfaces;
using SchoolSystem.Domain.Models.Entities;
using SchoolSystem.Domain.Models.Enums;
using SchoolSystem.Domain.Models.ValueObjects;
using SchoolSystem.Infrastructure.Data.Interfaces;
using SchoolSystem.Infrastructure.Services;

namespace SchoolSystem.Application.Features.Schedules.Commands;

[RequireRoles(UserRole.Admin, UserRole.SuperAdmin)]
public record CreateScheduleCommand(
    Guid      CourseId,
    Guid      RoomId,
    Guid      TeacherUserId,
    DayOfWeek DayOfWeek,
    TimeOnly  StartTime,
    TimeOnly  EndTime,
    DateOnly  EffectiveFrom,
    DateOnly  EffectiveTo) : ICommand<Guid>;

public class CreateScheduleCommandValidator : AbstractValidator<CreateScheduleCommand>
{
    public CreateScheduleCommandValidator()
    {
        RuleFor(x => x.CourseId).NotEmpty();
        RuleFor(x => x.RoomId).NotEmpty();
        RuleFor(x => x.TeacherUserId).NotEmpty();
        RuleFor(x => x.EndTime).GreaterThan(x => x.StartTime);
        RuleFor(x => x.EffectiveTo).GreaterThan(x => x.EffectiveFrom);
    }
}

public class CreateScheduleCommandHandler : IRequestHandler<CreateScheduleCommand, Guid>
{
    private readonly IUnitOfWork          _unitOfWork;
    private readonly ICurrentSchoolContext _context;

    public CreateScheduleCommandHandler(IUnitOfWork unitOfWork, ICurrentSchoolContext context)
    {
        _unitOfWork = unitOfWork;
        _context    = context;
    }

    public async Task<Guid> Handle(CreateScheduleCommand request, CancellationToken ct)
    {
        var startTs    = request.StartTime.ToTimeSpan();
        var endTs      = request.EndTime.ToTimeSpan();
        var fromDt     = request.EffectiveFrom.ToDateTime(TimeOnly.MinValue);
        var toDt       = request.EffectiveTo.ToDateTime(TimeOnly.MinValue);
        var dayOfWeek  = (int)request.DayOfWeek;

        var roomConflict = await _unitOfWork.Schedules.HasRoomConflictAsync(
            request.RoomId, dayOfWeek, startTs, endTs, fromDt, toDt, ct);
        if (roomConflict)
            throw new ConflictException("Room is already booked at this time.");

        var teacherConflict = await _unitOfWork.Schedules.HasTeacherConflictAsync(
            request.TeacherUserId, dayOfWeek, startTs, endTs, fromDt, toDt, ct);
        if (teacherConflict)
            throw new ConflictException("Teacher already has a schedule at this time.");

        var period   = new DateRange(request.EffectiveFrom, request.EffectiveTo);
        var schedule = Schedule.Create(request.CourseId, _context.SchoolId,
                                       request.RoomId, request.TeacherUserId,
                                       request.DayOfWeek, request.StartTime,
                                       request.EndTime, period);
        await _unitOfWork.Schedules.AddAsync(schedule, ct);
        return schedule.ScheduleId;
    }
}
