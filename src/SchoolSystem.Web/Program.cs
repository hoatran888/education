using Azure.Identity;
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

// ── Application + Infrastructure ────────────────────────────────────────────
builder.Services.AddInfrastructure(builder.Configuration, builder.Environment);
builder.Services.AddApplication();

// ── Override ICurrentSchoolContext for Blazor Server ────────────────────────
// CircuitUserService is populated by SchoolComponentBase before any MediatR calls.
// Registered last so it takes precedence over Infrastructure's CurrentSchoolContext.
builder.Services.AddScoped<CircuitUserService>();
builder.Services.AddScoped<ICurrentSchoolContext>(
    sp => sp.GetRequiredService<CircuitUserService>());

// ── Authentication (Azure AD B2C) ────────────────────────────────────────────
builder.Services
    .AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApp(builder.Configuration.GetSection("AzureAdB2C"));

builder.Services.AddControllersWithViews()
    .AddMicrosoftIdentityUI();

// ── Blazor Server + MudBlazor ────────────────────────────────────────────────
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor()
    .AddMicrosoftIdentityConsentHandler();
builder.Services.AddMudServices();

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
app.UseAuthorization();

app.MapControllers();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

if (app.Environment.IsDevelopment())
    await SchoolSystem.Infrastructure.Data.Seed.DatabaseSeeder.SeedAsync(app.Services);

app.Run();
