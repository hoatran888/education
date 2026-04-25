using MediatR;
using SchoolSystem.Application.Common.Interfaces;
using SchoolSystem.Application.Features.Enrollments.DTOs;
using SchoolSystem.Infrastructure.Data.Interfaces;

namespace SchoolSystem.Application.Features.Enrollments.Queries;

public record GetEnrollmentsByCourseQuery(Guid CourseId) : IQuery<IReadOnlyList<EnrollmentDto>>;

public class GetEnrollmentsByCourseQueryHandler
    : IRequestHandler<GetEnrollmentsByCourseQuery, IReadOnlyList<EnrollmentDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetEnrollmentsByCourseQueryHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<IReadOnlyList<EnrollmentDto>> Handle(
        GetEnrollmentsByCourseQuery request, CancellationToken ct)
    {
        var enrollments = await _unitOfWork.Enrollments.GetByCourseAsync(request.CourseId, ct);
        return enrollments.Select(GetEnrollmentsByStudentQueryHandler.ToDto).ToList();
    }
}
