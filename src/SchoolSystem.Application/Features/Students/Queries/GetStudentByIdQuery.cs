using MediatR;
using SchoolSystem.Application.Common.Exceptions;
using SchoolSystem.Application.Common.Interfaces;
using SchoolSystem.Application.Features.Students.DTOs;
using SchoolSystem.Domain.Models.Entities;
using SchoolSystem.Infrastructure.Data.Interfaces;

namespace SchoolSystem.Application.Features.Students.Queries;

public record GetStudentByIdQuery(Guid StudentProfileId) : IQuery<StudentDto>;

public class GetStudentByIdQueryHandler : IRequestHandler<GetStudentByIdQuery, StudentDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetStudentByIdQueryHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<StudentDto> Handle(GetStudentByIdQuery request, CancellationToken ct)
    {
        var profile = await _unitOfWork.Students.GetByIdAsync(request.StudentProfileId, ct)
            ?? throw new NotFoundException(nameof(StudentProfile), request.StudentProfileId);

        var user = await _unitOfWork.Users.GetByIdAsync(profile.UserId, ct);
        return ToDto(profile, user);
    }

    internal static StudentDto ToDto(StudentProfile p, User? u) => new(
        p.StudentProfileId, p.UserId, p.SchoolId,
        u?.FullName ?? string.Empty,
        u?.Email    ?? string.Empty,
        p.BirthDate, p.Age, p.GradeLevel,
        p.AdmissionDate, p.IsActive);
}
