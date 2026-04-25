using MediatR;
using SchoolSystem.Application.Common.Exceptions;
using SchoolSystem.Application.Common.Interfaces;
using SchoolSystem.Application.Features.Teachers.DTOs;
using SchoolSystem.Domain.Models.Entities;
using SchoolSystem.Infrastructure.Data.Interfaces;

namespace SchoolSystem.Application.Features.Teachers.Queries;

public record GetTeacherByIdQuery(Guid TeacherProfileId) : IQuery<TeacherDto>;

public class GetTeacherByIdQueryHandler : IRequestHandler<GetTeacherByIdQuery, TeacherDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetTeacherByIdQueryHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<TeacherDto> Handle(GetTeacherByIdQuery request, CancellationToken ct)
    {
        var profile = await _unitOfWork.Teachers.GetByIdAsync(request.TeacherProfileId, ct)
            ?? throw new NotFoundException(nameof(TeacherProfile), request.TeacherProfileId);

        var user = await _unitOfWork.Users.GetByIdAsync(profile.UserId, ct);
        return ToDto(profile, user);
    }

    internal static TeacherDto ToDto(TeacherProfile p, User? u) => new(
        p.TeacherProfileId, p.UserId, p.SchoolId,
        u?.FullName ?? string.Empty,
        u?.Email    ?? string.Empty,
        p.Degree, p.Specialization, p.HireDate, p.IsActive);
}
