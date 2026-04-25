using MediatR;
using SchoolSystem.Application.Common.Exceptions;
using SchoolSystem.Application.Common.Interfaces;
using SchoolSystem.Domain.Models.Enums;
using SchoolSystem.Infrastructure.Data.Interfaces;
using SchoolSystem.Infrastructure.Services;

namespace SchoolSystem.Application.Common.Behaviors;

public class AuthorizationBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly ICurrentSchoolContext _context;
    private readonly IUnitOfWork          _unitOfWork;

    public AuthorizationBehavior(ICurrentSchoolContext context, IUnitOfWork unitOfWork)
    {
        _context    = context;
        _unitOfWork = unitOfWork;
    }

    public async Task<TResponse> Handle(
        TRequest                          request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken                 ct)
    {
        var attr = typeof(TRequest)
            .GetCustomAttributes(typeof(RequireRolesAttribute), inherit: false)
            .Cast<RequireRolesAttribute>()
            .FirstOrDefault();

        if (attr is null)
            return await next();

        if (!_context.IsAuthenticated)
            throw new ForbiddenException("Authentication is required.");

        var user = await _unitOfWork.Users.GetByIdAsync(_context.UserId, ct)
            ?? throw new ForbiddenException("User not found.");

        if (user.IsInRole(UserRole.SuperAdmin))
            return await next();

        if (!attr.Roles.Any(r => user.IsInRole(r)))
            throw new ForbiddenException(
                $"Access denied. Required roles: {string.Join(", ", attr.Roles)}.");

        return await next();
    }
}
