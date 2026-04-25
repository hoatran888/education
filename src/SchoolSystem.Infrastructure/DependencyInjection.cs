using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SchoolSystem.Infrastructure.Data.Context;
using SchoolSystem.Infrastructure.Data.Interfaces;
using SchoolSystem.Infrastructure.Data.UnitOfWork;
using SchoolSystem.Infrastructure.Services;

namespace SchoolSystem.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration          configuration,
        IHostEnvironment?       environment = null)
    {
        var isDev = environment?.IsDevelopment() ?? false;

        // ── EF Core DbContext (scoped per request) ───────────────
        // Development: SQLite (no SQL Server install needed)
        // Production:  Azure SQL Server
        services.AddDbContext<SchoolDataContext>(options =>
        {
            if (isDev)
                options.UseSqlite(
                    configuration.GetConnectionString("SQLite")
                    ?? "Data Source=schoolsystem.db");
            else
            {
                var connectionString = configuration.GetConnectionString("AzureSQL")
                    ?? throw new InvalidOperationException(
                        "Connection string 'AzureSQL' is not configured.");
                options.UseSqlServer(connectionString);
            }
        });

        // ── Current school/user context from JWT claims ──────────
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentSchoolContext, CurrentSchoolContext>();

        // ── Unit of Work (scoped per request) ────────────────────
        services.AddScoped<IUnitOfWork>(sp =>
        {
            var context       = sp.GetRequiredService<SchoolDataContext>();
            var schoolContext = sp.GetRequiredService<ICurrentSchoolContext>();

            var schoolId = schoolContext.IsAuthenticated
                ? schoolContext.SchoolId
                : Guid.Empty;

            return new LinqUnitOfWork(context, schoolId);
        });

        return services;
    }
}
