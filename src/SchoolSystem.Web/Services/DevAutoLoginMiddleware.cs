using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using SchoolSystem.Infrastructure.Data.Seed;

namespace SchoolSystem.Web.Services;

/// <summary>
/// Auto-signs in a dev admin user when B2C is not configured.
/// This only runs when the app is in local dev mode (no real B2C tenant).
/// </summary>
public class DevAutoLoginMiddleware
{
    private readonly RequestDelegate _next;

    public DevAutoLoginMiddleware(RequestDelegate next) => _next = next;

    public async Task InvokeAsync(HttpContext context)
    {
        if (!context.User.Identity?.IsAuthenticated ?? true)
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, DatabaseSeeder.DevAdminUserId.ToString()),
                new("sub",                     DatabaseSeeder.DevAdminUserId.ToString()),
                new(ClaimTypes.Name,           "Admin User"),
                new(ClaimTypes.Email,          "admin@greenwood.edu"),
                new("email",                   "admin@greenwood.edu"),
                new("school_id",               DatabaseSeeder.DevSchoolId.ToString()),
            };
            var identity  = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
            context.User = principal;
        }

        await _next(context);
    }
}
