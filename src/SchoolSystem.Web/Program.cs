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

// ── Detect whether real Entra External ID is configured ─────────────────────
// ClientId must be a real GUID (not all-zeros placeholder)
var entraClientId = builder.Configuration["AzureAd:ClientId"];
var isRealEntra   = Guid.TryParse(entraClientId, out var parsedId)
                 && parsedId != Guid.Empty;

// ── Application + Infrastructure ────────────────────────────────────────────
builder.Services.AddInfrastructure(builder.Configuration, builder.Environment);
builder.Services.AddApplication();

// ── Override ICurrentSchoolContext for Blazor Server ────────────────────────
builder.Services.AddScoped<CircuitUserService>();
builder.Services.AddScoped<ICurrentSchoolContext>(
    sp => sp.GetRequiredService<CircuitUserService>());

// ── Authentication ────────────────────────────────────────────────────────────
if (isRealEntra)
{
    // Production: Microsoft Entra External ID (replaces Azure AD B2C)
    builder.Services
        .AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
        .AddMicrosoftIdentityWebApp(builder.Configuration.GetSection("AzureAd"));

    builder.Services.AddControllersWithViews()
        .AddMicrosoftIdentityUI();

    builder.Services.AddServerSideBlazor()
        .AddMicrosoftIdentityConsentHandler();
}
else
{
    // Local dev: simple cookie auth with dev-login page
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
builder.Services.AddHealthChecks();

// ────────────────────────────────────────────────────────────────────────────
var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();

// Dev-login redirect middleware — only active when Entra is not configured
if (!isRealEntra)
    app.UseMiddleware<DevAutoLoginMiddleware>();

app.UseAuthorization();
app.MapControllers();
app.MapBlazorHub();
app.MapHealthChecks("/health");
app.MapFallbackToPage("/_Host");

await SchoolSystem.Infrastructure.Data.Seed.DatabaseSeeder.SeedAsync(app.Services);

app.Run();
