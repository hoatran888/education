using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolSystem.Domain.Models.Enums;
using SchoolSystem.Infrastructure.Data.Context;

namespace SchoolSystem.Web.Controllers;

[Route("dev-login")]
public class DevLoginController : Controller
{
    private readonly SchoolDataContext _db;

    public DevLoginController(SchoolDataContext db) => _db = db;

    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        var users = await _db.Users
            .Include(u => u.UserRoles)
            .Where(u => u.IsActive)
            .OrderBy(u => u.FirstName)
            .ToListAsync();

        var sb = new StringBuilder();
        sb.Append("""
            <!DOCTYPE html>
            <html>
            <head>
              <title>Dev Login — SchoolSystem</title>
              <meta charset="utf-8" />
              <style>
                body { font-family: -apple-system, BlinkMacSystemFont, "Segoe UI", sans-serif;
                       max-width: 640px; margin: 60px auto; padding: 0 20px; background: #f5f5f5; }
                h1   { color: #1565c0; margin-bottom: 4px; }
                .sub { color: #666; margin-bottom: 24px; font-size: 14px; }
                .warn { background: #fff8e1; border-left: 4px solid #ffc107;
                        padding: 10px 14px; border-radius: 4px; margin-bottom: 24px;
                        font-size: 13px; color: #5d4037; }
                .card { background: white; border-radius: 8px; padding: 16px 20px;
                        margin-bottom: 10px; display: flex; justify-content: space-between;
                        align-items: center; box-shadow: 0 1px 3px rgba(0,0,0,.12); }
                .name { font-weight: 600; font-size: 15px; color: #212121; }
                .meta { font-size: 13px; color: #757575; margin-top: 2px; }
                .badge { display: inline-block; padding: 2px 8px; border-radius: 12px;
                         font-size: 12px; font-weight: 500; margin-right: 4px; }
                .r-Admin,.r-SuperAdmin { background: #e3f2fd; color: #1565c0; }
                .r-Teacher             { background: #e8f5e9; color: #2e7d32; }
                .r-Student             { background: #fce4ec; color: #880e4f; }
                .r-Parent              { background: #fff3e0; color: #e65100; }
                .r-default             { background: #ede7f6; color: #4527a0; }
                .btn { background: #1976d2; color: white; border: none; padding: 8px 18px;
                       border-radius: 6px; font-size: 14px; cursor: pointer;
                       text-decoration: none; white-space: nowrap; }
                .btn:hover { background: #1565c0; }
              </style>
            </head>
            <body>
              <h1>Development Login</h1>
              <p class="sub">SchoolSystem · Local Dev Mode</p>
              <div class="warn">⚠️ This page is only visible in development. Choose a user to sign in as.</div>
            """);

        foreach (var user in users)
        {
            var badges = user.UserRoles
                .Select(r => ((UserRole)r.Role).ToString())
                .Select(name =>
                {
                    var cls = name is "Admin" or "SuperAdmin" ? $"r-{name}"
                            : name == "Teacher"  ? "r-Teacher"
                            : name == "Student"  ? "r-Student"
                            : name == "Parent"   ? "r-Parent"
                            : "r-default";
                    return $"<span class=\"badge {cls}\">{name}</span>";
                });

            sb.Append($"""
                <div class="card">
                  <div>
                    <div class="name">{user.FirstName} {user.LastName}</div>
                    <div class="meta">{string.Join("", badges)} &nbsp;{user.Email}</div>
                  </div>
                  <a class="btn" href="/dev-login/as?userId={user.UserId}">Sign In</a>
                </div>
                """);
        }

        sb.Append("</body></html>");
        return Content(sb.ToString(), "text/html");
    }

    [HttpGet("as")]
    public async Task<IActionResult> SignInAs(Guid userId, string? returnUrl = null)
    {
        var user = await _db.Users
            .Include(u => u.UserRoles)
            .FirstOrDefaultAsync(u => u.UserId == userId && u.IsActive);

        if (user is null) return NotFound("User not found.");

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new("sub",                     user.UserId.ToString()),
            new(ClaimTypes.Name,           $"{user.FirstName} {user.LastName}"),
            new(ClaimTypes.Email,          user.Email),
            new("email",                   user.Email),
            new("school_id",               user.SchoolId.ToString()),
        };

        foreach (var ur in user.UserRoles)
            claims.Add(new Claim(ClaimTypes.Role, ((UserRole)ur.Role).ToString()));

        var identity  = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

        var target = !string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl)
            ? returnUrl
            : "/";
        return LocalRedirect(target);
    }

    [HttpGet("signout")]
    public async Task<IActionResult> SignOutDev()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction(nameof(Index));
    }
}
