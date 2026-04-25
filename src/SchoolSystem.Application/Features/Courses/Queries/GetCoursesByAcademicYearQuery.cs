using MediatR;
using SchoolSystem.Application.Common.Interfaces;
using SchoolSystem.Application.Features.Courses.DTOs;
using SchoolSystem.Infrastructure.Data.Interfaces;

namespace SchoolSystem.Application.Features.Courses.Queries;

public record GetCoursesByAcademicYearQuery(Guid AcademicYearId) : IQuery<IReadOnlyList<CourseDto>>;

public class GetCoursesByAcademicYearQueryHandler
    : IRequestHandler<GetCoursesByAcademicYearQuery, IReadOnlyList<CourseDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetCoursesByAcademicYearQueryHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<IReadOnlyList<CourseDto>> Handle(
        GetCoursesByAcademicYearQuery request, CancellationToken ct)
    {
        var courses = await _unitOfWork.Courses.GetByAcademicYearAsync(request.AcademicYearId, ct);
        return courses.Select(GetCourseByIdQueryHandler.ToDto).ToList();
    }
}
