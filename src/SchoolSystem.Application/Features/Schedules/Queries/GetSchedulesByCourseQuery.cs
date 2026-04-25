using MediatR;
using SchoolSystem.Application.Common.Interfaces;
using SchoolSystem.Application.Features.Schedules.DTOs;
using SchoolSystem.Domain.Models.Entities;
using SchoolSystem.Infrastructure.Data.Interfaces;

namespace SchoolSystem.Application.Features.Schedules.Queries;

public record GetSchedulesByCourseQuery(Guid CourseId) : IQuery<IReadOnlyList<ScheduleDto>>;

public class GetSchedulesByCourseQueryHandler
    : IRequestHandler<GetSchedulesByCourseQuery, IReadOnlyList<ScheduleDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetSchedulesByCourseQueryHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<IReadOnlyList<ScheduleDto>> Handle(
        GetSchedulesByCourseQuery request, CancellationToken ct)
    {
        var schedules = await _unitOfWork.Schedules.GetByCourseAsync(request.CourseId, ct);
        return schedules.Select(ToDto).ToList();
    }

    internal static ScheduleDto ToDto(Schedule s) => new(
        s.ScheduleId, s.CourseId, s.SchoolId, s.RoomId, s.TeacherUserId,
        s.DayOfWeek.ToString(), s.StartTime, s.EndTime,
        s.EffectivePeriod.Start, s.EffectivePeriod.End,
        s.CourseName, s.RoomName, s.TeacherName);
}
