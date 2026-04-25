using Azure.Identity;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using MudBlazor.Services;
using SchoolSystem.Application;
using SchoolSystem.Infrastructure;
using SchoolSystem.Infrastructure.Services;
using SchoolSystem.Web.Services;

var builder = WebApplication.CreateBuilder(args);

// ── Azure Key Vault ─────────────────────────────────────────────────────────
var kvUri = builder.Configuration["KeyVault:Uri"];
if (!string.IsNullOrWhiteSpace(kvUri) && kvUri.StartsWith("https://") && !kvUri.Contains("your-keyvault"))
    builder.Configuration.AddAzureKeyVault(new Uri(kvUri), new DefaultAzureCredential());

// ── Detect whether real B2C is configured ────────────────────────────────────
var b2cClientId = builder.Configuration["AzureAdB2C:ClientId"];
var isRealB2C   = !string.IsNullOrWhiteSpace(b2cClientId)
               && b2cClientId != "00000000-0000-0000-0000-000000000000";

// ── Application + Infrastructure ────────────────────────────────────────────
builder.Services.AddInfrastructure(builder.Configuration, builder.Environment);
builder.Services.AddApplication();

// ── Override ICurrentSchoolContext for Blazor Server ────────────────────────
builder.Services.AddScoped<CircuitUserService>();
builder.Services.AddScoped<ICurrentSchoolContext>(
    sp => sp.GetRequiredService<CircuitUserService>());

// ── Authentication ────────────────────────────────────────────────────────────
if (isRealB2C)
{
    // Production: full Azure AD B2C
    builder.Services
        .AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
        .AddMicrosoftIdentityWebApp(builder.Configuration.GetSection("AzureAdB2C"));

    builder.Services.AddControllersWithViews()
        .AddMicrosoftIdentityUI();

    builder.Services.AddServerSideBlazor()
        .AddMicrosoftIdentityConsentHandler();
}
else
{
    // Local dev: simple cookie auth with auto-login middleware
    builder.Services
        .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
        .AddCookie(options =>
        {
            options.Cookie.Name    = "SchoolSystem.DevAuth";
            options.ExpireTimeSpan = TimeSpan.FromDays(1);
        });

    builder.Services.AddControllersWithViews();
    builder.Services.AddServerSideBlazor();
}

builder.Services.AddRazorPages();
builder.Services.AddMudServices();

// ────────────────────────────────────────────────────────────────────────────
var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();

// Auto-sign in dev admin when B2C is not configured
if (!isRealB2C)
    app.UseMiddleware<DevAutoLoginMiddleware>();

app.UseAuthorization();
app.MapControllers();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

await SchoolSystem.Infrastructure.Data.Seed.DatabaseSeeder.SeedAsync(app.Services);

app.Run();
