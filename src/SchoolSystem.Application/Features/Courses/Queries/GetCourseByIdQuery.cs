using MediatR;
using SchoolSystem.Application.Common.Exceptions;
using SchoolSystem.Application.Common.Interfaces;
using SchoolSystem.Application.Features.Courses.DTOs;
using SchoolSystem.Domain.Models.Entities;
using SchoolSystem.Infrastructure.Data.Interfaces;

namespace SchoolSystem.Application.Features.Courses.Queries;

public record GetCourseByIdQuery(Guid CourseId) : IQuery<CourseDto>;

public class GetCourseByIdQueryHandler : IRequestHandler<GetCourseByIdQuery, CourseDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetCourseByIdQueryHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<CourseDto> Handle(GetCourseByIdQuery request, CancellationToken ct)
    {
        var course = await _unitOfWork.Courses.GetByIdAsync(request.CourseId, ct)
            ?? throw new NotFoundException(nameof(Course), request.CourseId);
        return ToDto(course);
    }

    internal static CourseDto ToDto(Course c) => new(
        c.CourseId, c.SchoolId, c.AcademicYearId,
        c.Name, c.Description,
        c.GradeLevel, c.Credits, c.MaxStudents,
        c.ActiveEnrollmentCount,
        c.TeacherUserId,
        c.Status.ToString(),
        c.Duration.Start, c.Duration.End,
        c.CreatedAt);
}
