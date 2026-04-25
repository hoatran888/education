using MediatR;
using SchoolSystem.Application.Common.Interfaces;
using SchoolSystem.Application.Features.Users.DTOs;
using SchoolSystem.Domain.Models.Enums;
using SchoolSystem.Infrastructure.Data.Interfaces;

namespace SchoolSystem.Application.Features.Users.Queries;

public record GetUsersByRoleQuery(UserRole Role) : IQuery<IReadOnlyList<UserDto>>;

public class GetUsersByRoleQueryHandler : IRequestHandler<GetUsersByRoleQuery, IReadOnlyList<UserDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetUsersByRoleQueryHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<IReadOnlyList<UserDto>> Handle(GetUsersByRoleQuery request, CancellationToken ct)
    {
        var users = await _unitOfWork.Users.GetByRoleAsync(request.Role, ct);
        return users.Select(GetUserByIdQueryHandler.ToDto).ToList();
    }
}
