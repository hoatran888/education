using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolSystem.Domain.Models.Enums;
using SchoolSystem.Infrastructure.Data.Context;
using SchoolSystem.Infrastructure.Services;

namespace SchoolSystem.Web.Controllers;

[Route("account")]
public class AccountController : Controller
{
    private readonly SchoolDataContext _db;
    private readonly IPasswordHasher   _hasher;

    public AccountController(SchoolDataContext db, IPasswordHasher hasher)
    {
        _db     = db;
        _hasher = hasher;
    }

    [HttpGet("login")]
    public IActionResult Login(string? returnUrl = null) =>
        Content(BuildPage(returnUrl), "text/html");

    [HttpPost("login")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(
        string email, string password, string? returnUrl = null)
    {
        var user = await _db.Users
            .Include(u => u.UserRoles)
            .FirstOrDefaultAsync(u => u.Email == email.ToLowerInvariant() && u.IsActive);

        if (user is null || string.IsNullOrEmpty(user.PasswordHash) ||
            !_hasher.Verify(user.PasswordHash, password))
            return Content(BuildPage(returnUrl, "Invalid email or password."), "text/html");

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
            ? returnUrl : "/";
        return LocalRedirect(target);
    }

    [HttpGet("logout")]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return Redirect("/account/login");
    }

    private string BuildPage(string? returnUrl, string? error = null)
    {
        var token  = HttpContext.RequestServices
            .GetRequiredService<Microsoft.AspNetCore.Antiforgery.IAntiforgery>()
            .GetAndStoreTokens(HttpContext).RequestToken;

        var errorHtml = error is null ? "" : $"""
            <div class="error">{System.Net.WebUtility.HtmlEncode(error)}</div>
            """;

        return $$"""
            <!DOCTYPE html>
            <html lang="en">
            <head>
              <meta charset="utf-8" />
              <meta name="viewport" content="width=device-width, initial-scale=1" />
              <title>Sign In — School Management System</title>
              <style>
                *, *::before, *::after { box-sizing: border-box; margin: 0; padding: 0; }
                body {
                  font-family: -apple-system, BlinkMacSystemFont, "Segoe UI", Roboto, sans-serif;
                  background: #1565c0;
                  min-height: 100vh;
                  display: flex;
                  align-items: center;
                  justify-content: center;
                  padding: 16px;
                }
                .card {
                  background: white;
                  border-radius: 12px;
                  padding: 40px 36px;
                  width: 100%;
                  max-width: 400px;
                  box-shadow: 0 8px 32px rgba(0,0,0,.24);
                }
                .logo {
                  text-align: center;
                  margin-bottom: 28px;
                }
                .logo-icon {
                  width: 56px; height: 56px;
                  background: #1565c0;
                  border-radius: 50%;
                  display: inline-flex;
                  align-items: center;
                  justify-content: center;
                  margin-bottom: 12px;
                }
                .logo-icon svg { fill: white; }
                h1 { font-size: 20px; font-weight: 600; color: #212121; text-align: center; }
                p.sub { font-size: 13px; color: #757575; text-align: center; margin-top: 4px; }
                .field { margin-top: 20px; }
                label { display: block; font-size: 13px; color: #424242; margin-bottom: 6px; font-weight: 500; }
                input[type=email], input[type=password] {
                  width: 100%; padding: 10px 14px;
                  border: 1px solid #bdbdbd; border-radius: 6px;
                  font-size: 15px; outline: none; transition: border-color .2s;
                }
                input:focus { border-color: #1565c0; box-shadow: 0 0 0 3px rgba(21,101,192,.15); }
                .btn {
                  display: block; width: 100%; margin-top: 28px;
                  padding: 12px; background: #1565c0; color: white;
                  border: none; border-radius: 6px; font-size: 15px;
                  font-weight: 600; cursor: pointer; transition: background .2s;
                }
                .btn:hover { background: #0d47a1; }
                .error {
                  margin-top: 16px; padding: 10px 14px;
                  background: #ffebee; border: 1px solid #ef9a9a;
                  border-radius: 6px; color: #c62828; font-size: 13px;
                }
                .hint {
                  margin-top: 20px; padding: 10px 14px;
                  background: #e3f2fd; border-radius: 6px;
                  font-size: 12px; color: #1565c0; text-align: center;
                }
              </style>
            </head>
            <body>
              <div class="card">
                <div class="logo">
                  <div class="logo-icon">
                    <svg xmlns="http://www.w3.org/2000/svg" height="28" width="28" viewBox="0 0 24 24">
                      <path d="M12 3L1 9l11 6 9-4.91V17h2V9L12 3zM5 13.18v4L12 21l7-3.82v-4L12 17l-7-3.82z"/>
                    </svg>
                  </div>
                  <h1>School Management System</h1>
                  <p class="sub">Sign in to continue</p>
                </div>

                <form method="post" action="/account/login{{(returnUrl is null ? "" : $"?returnUrl={Uri.EscapeDataString(returnUrl)}")}}">
                  <input type="hidden" name="__RequestVerificationToken" value="{{token}}" />

                  <div class="field">
                    <label for="email">Email address</label>
                    <input id="email" name="email" type="email" autocomplete="email"
                           placeholder="you@school.edu" required autofocus />
                  </div>

                  <div class="field">
                    <label for="password">Password</label>
                    <input id="password" name="password" type="password"
                           autocomplete="current-password" placeholder="••••••••" required />
                  </div>

                  {{errorHtml}}

                  <button class="btn" type="submit">Sign In</button>
                </form>

                <div class="hint">
                  Dev accounts — all passwords: <strong>School123!</strong><br/>
                  admin@greenwood.edu · alice@greenwood.edu · frank@example.com
                </div>
              </div>
            </body>
            </html>
            """;
    }
}
