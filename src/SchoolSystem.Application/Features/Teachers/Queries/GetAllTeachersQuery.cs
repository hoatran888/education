using MediatR;
using SchoolSystem.Application.Common.Interfaces;
using SchoolSystem.Application.Features.Teachers.DTOs;
using SchoolSystem.Infrastructure.Data.Interfaces;

namespace SchoolSystem.Application.Features.Teachers.Queries;

public record GetAllTeachersQuery : IQuery<IReadOnlyList<TeacherDto>>;

public class GetAllTeachersQueryHandler : IRequestHandler<GetAllTeachersQuery, IReadOnlyList<TeacherDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetAllTeachersQueryHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<IReadOnlyList<TeacherDto>> Handle(GetAllTeachersQuery request, CancellationToken ct)
    {
        var profiles = await _unitOfWork.Teachers.GetAllAsync(ct);
        var result   = new List<TeacherDto>(profiles.Count);
        foreach (var p in profiles)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(p.UserId, ct);
            result.Add(GetTeacherByIdQueryHandler.ToDto(p, user));
        }
        return result;
    }
}
