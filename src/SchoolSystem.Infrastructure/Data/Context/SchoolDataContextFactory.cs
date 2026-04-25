using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace SchoolSystem.Infrastructure.Data.Context;

// Used only by dotnet-ef CLI at design time (migrations).
// Always uses SQLite so no SQL Server is needed on dev machines.
public class SchoolDataContextFactory : IDesignTimeDbContextFactory<SchoolDataContext>
{
    public SchoolDataContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<SchoolDataContext>()
            .UseSqlite("Data Source=schoolsystem_migrations.db")
            .Options;

        return new SchoolDataContext(options);
    }
}
