using MediatR;
using SchoolSystem.Application.Common.Interfaces;
using SchoolSystem.Application.Features.Students.DTOs;
using SchoolSystem.Infrastructure.Data.Interfaces;

namespace SchoolSystem.Application.Features.Students.Queries;

public record GetStudentsByGradeLevelQuery(int GradeLevel) : IQuery<IReadOnlyList<StudentDto>>;

public class GetStudentsByGradeLevelQueryHandler
    : IRequestHandler<GetStudentsByGradeLevelQuery, IReadOnlyList<StudentDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetStudentsByGradeLevelQueryHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<IReadOnlyList<StudentDto>> Handle(
        GetStudentsByGradeLevelQuery request, CancellationToken ct)
    {
        var profiles = await _unitOfWork.Students.GetByGradeLevelAsync(request.GradeLevel, ct);
        var result   = new List<StudentDto>(profiles.Count);
        foreach (var p in profiles)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(p.UserId, ct);
            result.Add(GetStudentByIdQueryHandler.ToDto(p, user));
        }
        return result;
    }
}
