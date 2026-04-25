using MediatR;
using SchoolSystem.Application.Common.Exceptions;
using SchoolSystem.Application.Common.Interfaces;
using SchoolSystem.Application.Features.Schools.DTOs;
using SchoolSystem.Domain.Models.Entities;
using SchoolSystem.Infrastructure.Data.Interfaces;

namespace SchoolSystem.Application.Features.Schools.Queries;

public record GetSchoolByIdQuery(Guid SchoolId) : IQuery<SchoolDto>;

public class GetSchoolByIdQueryHandler : IRequestHandler<GetSchoolByIdQuery, SchoolDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetSchoolByIdQueryHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<SchoolDto> Handle(GetSchoolByIdQuery request, CancellationToken ct)
    {
        var school = await _unitOfWork.Schools.GetByIdAsync(request.SchoolId, ct)
            ?? throw new NotFoundException(nameof(School), request.SchoolId);

        return ToDto(school);
    }

    internal static SchoolDto ToDto(School s) => new(
        s.SchoolId, s.Name,
        s.Address.Street, s.Address.City, s.Address.State,
        s.Address.ZipCode, s.Address.Country,
        s.Contact.Email, s.Contact.Phone,
        s.LogoUrl, s.IsActive, s.CreatedAt);
}
