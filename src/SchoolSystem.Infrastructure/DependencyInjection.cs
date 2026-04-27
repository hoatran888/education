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
        var azureSql = configuration.GetConnectionString("AzureSQL");
        var hasRealSqlServer = !string.IsNullOrWhiteSpace(azureSql)
                               && !azureSql.Contains("your-server");

        // Use SQLite when: explicitly in Development, OR no real SQL Server is configured.
        // Use SQL Server only when a real AzureSQL connection string is present.
        services.AddDbContext<SchoolDataContext>(options =>
        {
            if (hasRealSqlServer && !isDev)
            {
                options.UseSqlServer(azureSql!);
            }
            else
            {
                // Use absolute path so the .db file location is predictable
                // regardless of the working directory used to launch the app.
                // Azure App Service: /home persists across deployments.
                // Local dev: next to the Web project's content root.
                string defaultConnStr;
                if (environment?.IsProduction() == true)
                {
                    defaultConnStr = "Data Source=/home/schoolsystem.db";
                }
                else
                {
                    var dbPath = Path.Combine(
                        environment?.ContentRootPath ?? AppContext.BaseDirectory,
                        "schoolsystem.db");
                    defaultConnStr = $"Data Source={dbPath}";
                }

                var connStr = configuration.GetConnectionString("SQLite") ?? defaultConnStr;
                options.UseSqlite(connStr);
            }
        });

        // ── Password hashing ────────────────────────────────────
        services.AddSingleton<IPasswordHasher, AspNetPasswordHasher>();

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
