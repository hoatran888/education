using System.Security.Claims;
using SchoolSystem.Infrastructure.Services;

namespace SchoolSystem.Web.Services;

/// <summary>
/// Blazor Server replacement for CurrentSchoolContext.
/// Populated from AuthenticationStateProvider during circuit initialization
/// via SchoolComponentBase, before any MediatR calls are made.
/// </summary>
public class CircuitUserService : ICurrentSchoolContext
{
    private ClaimsPrincipal _user = new(new ClaimsIdentity());

    public void SetUser(ClaimsPrincipal user) => _user = user;

    public bool IsAuthenticated => _user.Identity?.IsAuthenticated ?? false;

    public Guid SchoolId
    {
        get
        {
            var claim = _user.FindFirstValue("school_id")
                ?? throw new InvalidOperationException("school_id claim is missing.");
            return Guid.Parse(claim);
        }
    }

    public Guid UserId
    {
        get
        {
            var claim = _user.FindFirstValue(ClaimTypes.NameIdentifier)
                     ?? _user.FindFirstValue("sub")
                     ?? throw new InvalidOperationException("User identity claim is missing.");
            return Guid.Parse(claim);
        }
    }

    public string Email =>
        _user.FindFirstValue(ClaimTypes.Email)
        ?? _user.FindFirstValue("email")
        ?? throw new InvalidOperationException("Email claim is missing.");
}
