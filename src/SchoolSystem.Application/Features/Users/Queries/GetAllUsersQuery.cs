using MediatR;
using SchoolSystem.Application.Common.Interfaces;
using SchoolSystem.Application.Features.Users.DTOs;
using SchoolSystem.Infrastructure.Data.Interfaces;

namespace SchoolSystem.Application.Features.Users.Queries;

public record GetAllUsersQuery : IQuery<IReadOnlyList<UserDto>>;

public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, IReadOnlyList<UserDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetAllUsersQueryHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<IReadOnlyList<UserDto>> Handle(GetAllUsersQuery request, CancellationToken ct)
    {
        var users = await _unitOfWork.Users.GetAllAsync(ct);
        return users.Select(GetUserByIdQueryHandler.ToDto).ToList();
    }
}
