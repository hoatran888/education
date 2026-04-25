using MediatR;
using SchoolSystem.Application.Common.Interfaces;
using SchoolSystem.Application.Features.Courses.DTOs;
using SchoolSystem.Infrastructure.Data.Interfaces;

namespace SchoolSystem.Application.Features.Courses.Queries;

public record GetOpenCoursesQuery : IQuery<IReadOnlyList<CourseDto>>;

public class GetOpenCoursesQueryHandler : IRequestHandler<GetOpenCoursesQuery, IReadOnlyList<CourseDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetOpenCoursesQueryHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<IReadOnlyList<CourseDto>> Handle(GetOpenCoursesQuery request, CancellationToken ct)
    {
        var courses = await _unitOfWork.Courses.GetOpenCoursesAsync(ct);
        return courses.Select(GetCourseByIdQueryHandler.ToDto).ToList();
    }
}
