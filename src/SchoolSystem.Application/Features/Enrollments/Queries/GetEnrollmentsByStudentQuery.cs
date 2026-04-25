using MediatR;
using SchoolSystem.Application.Common.Interfaces;
using SchoolSystem.Application.Features.Enrollments.DTOs;
using SchoolSystem.Domain.Models.Entities;
using SchoolSystem.Infrastructure.Data.Interfaces;

namespace SchoolSystem.Application.Features.Enrollments.Queries;

public record GetEnrollmentsByStudentQuery(Guid StudentUserId) : IQuery<IReadOnlyList<EnrollmentDto>>;

public class GetEnrollmentsByStudentQueryHandler
    : IRequestHandler<GetEnrollmentsByStudentQuery, IReadOnlyList<EnrollmentDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetEnrollmentsByStudentQueryHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<IReadOnlyList<EnrollmentDto>> Handle(
        GetEnrollmentsByStudentQuery request, CancellationToken ct)
    {
        var enrollments = await _unitOfWork.Enrollments.GetByStudentAsync(request.StudentUserId, ct);
        return enrollments.Select(ToDto).ToList();
    }

    internal static EnrollmentDto ToDto(Enrollment e) => new(
        e.EnrollmentId, e.CourseId, e.StudentUserId, e.SchoolId,
        e.EnrolledDate, e.Status.ToString(), e.DroppedDate, e.DropReason);
}
