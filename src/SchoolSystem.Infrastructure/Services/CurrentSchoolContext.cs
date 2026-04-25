using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace SchoolSystem.Infrastructure.Services;

public class CurrentSchoolContext : ICurrentSchoolContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentSchoolContext(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;

    public bool IsAuthenticated =>
        User?.Identity?.IsAuthenticated ?? false;

    public Guid SchoolId
    {
        get
        {
            var claim = User?.FindFirstValue("school_id")
                ?? throw new InvalidOperationException("school_id claim is missing from token.");
            return Guid.Parse(claim);
        }
    }

    public Guid UserId
    {
        get
        {
            var claim = User?.FindFirstValue(ClaimTypes.NameIdentifier)
                     ?? User?.FindFirstValue("sub")
                     ?? throw new InvalidOperationException("User identity claim is missing from token.");
            return Guid.Parse(claim);
        }
    }

    public string Email =>
        User?.FindFirstValue(ClaimTypes.Email)
        ?? User?.FindFirstValue("email")
        ?? throw new InvalidOperationException("Email claim is missing from token.");
}
