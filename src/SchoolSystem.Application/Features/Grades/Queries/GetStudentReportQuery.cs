using MediatR;
using SchoolSystem.Application.Common.Interfaces;
using SchoolSystem.Application.Features.Grades.DTOs;
using SchoolSystem.Infrastructure.Data.Interfaces;

namespace SchoolSystem.Application.Features.Grades.Queries;

public record GetStudentReportQuery(Guid StudentUserId) : IQuery<IReadOnlyList<GradeDto>>;

public class GetStudentReportQueryHandler
    : IRequestHandler<GetStudentReportQuery, IReadOnlyList<GradeDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetStudentReportQueryHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<IReadOnlyList<GradeDto>> Handle(
        GetStudentReportQuery request, CancellationToken ct)
    {
        var grades = await _unitOfWork.Grades.GetPublishedByStudentAsync(request.StudentUserId, ct);
        return grades.Select(GetGradesByEnrollmentQueryHandler.ToDto).ToList();
    }
}
