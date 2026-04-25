using MediatR;
using SchoolSystem.Application.Common.Interfaces;
using SchoolSystem.Application.Features.Schools.DTOs;
using SchoolSystem.Infrastructure.Data.Interfaces;

namespace SchoolSystem.Application.Features.Schools.Queries;

public record GetAllSchoolsQuery : IQuery<IReadOnlyList<SchoolDto>>;

public class GetAllSchoolsQueryHandler : IRequestHandler<GetAllSchoolsQuery, IReadOnlyList<SchoolDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetAllSchoolsQueryHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<IReadOnlyList<SchoolDto>> Handle(GetAllSchoolsQuery request, CancellationToken ct)
    {
        var schools = await _unitOfWork.Schools.GetAllAsync(ct);
        return schools.Select(GetSchoolByIdQueryHandler.ToDto).ToList();
    }
}
