using MediatR;
using SchoolSystem.Application.Common.Interfaces;
using SchoolSystem.Application.Features.Schedules.DTOs;
using SchoolSystem.Infrastructure.Data.Interfaces;

namespace SchoolSystem.Application.Features.Schedules.Queries;

public record GetScheduleGridQuery(int Month, int Year) : IQuery<IReadOnlyList<ScheduleDto>>;

public class GetScheduleGridQueryHandler
    : IRequestHandler<GetScheduleGridQuery, IReadOnlyList<ScheduleDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetScheduleGridQueryHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<IReadOnlyList<ScheduleDto>> Handle(
        GetScheduleGridQuery request, CancellationToken ct)
    {
        var schedules = await _unitOfWork.Schedules.GetByMonthAsync(request.Month, request.Year, ct);
        return schedules.Select(GetSchedulesByCourseQueryHandler.ToDto).ToList();
    }
}
