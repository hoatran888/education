using MediatR;
using SchoolSystem.Application.Common.Interfaces;
using SchoolSystem.Application.Features.Grades.DTOs;
using SchoolSystem.Domain.Models.Entities;
using SchoolSystem.Infrastructure.Data.Interfaces;

namespace SchoolSystem.Application.Features.Grades.Queries;

public record GetGradesByEnrollmentQuery(Guid EnrollmentId) : IQuery<IReadOnlyList<GradeDto>>;

public class GetGradesByEnrollmentQueryHandler
    : IRequestHandler<GetGradesByEnrollmentQuery, IReadOnlyList<GradeDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetGradesByEnrollmentQueryHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<IReadOnlyList<GradeDto>> Handle(
        GetGradesByEnrollmentQuery request, CancellationToken ct)
    {
        var grades = await _unitOfWork.Grades.GetByEnrollmentAsync(request.EnrollmentId, ct);
        return grades.Select(ToDto).ToList();
    }

    internal static GradeDto ToDto(Grade g) => new(
        g.GradeId, g.EnrollmentId, g.GradePeriodId, g.SchoolId, g.TeacherUserId,
        g.Score.Value, g.LetterGrade.ToString(), g.Comment,
        g.IsPublished, g.PublishedAt, g.GradedAt,
        g.PeriodName, g.CourseName);
}
