using MediatR;
using SchoolSystem.Application.Common.Exceptions;
using SchoolSystem.Application.Common.Interfaces;
using SchoolSystem.Application.Features.Users.DTOs;
using SchoolSystem.Domain.Models.Entities;
using SchoolSystem.Infrastructure.Data.Interfaces;

namespace SchoolSystem.Application.Features.Users.Queries;

public record GetUserByIdQuery(Guid UserId) : IQuery<UserDto>;

public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, UserDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetUserByIdQueryHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<UserDto> Handle(GetUserByIdQuery request, CancellationToken ct)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(request.UserId, ct)
            ?? throw new NotFoundException(nameof(User), request.UserId);
        return ToDto(user);
    }

    internal static UserDto ToDto(User u) => new(
        u.UserId, u.SchoolId,
        u.FirstName, u.LastName, u.FullName,
        u.Email, u.Phone, u.Sex.ToString(),
        u.Address.Street, u.Address.City, u.Address.State,
        u.Address.ZipCode, u.Address.Country,
        u.PhotoUrl, u.IsActive, u.CreatedAt,
        u.Roles.Select(r => r.ToString()).ToList());
}
