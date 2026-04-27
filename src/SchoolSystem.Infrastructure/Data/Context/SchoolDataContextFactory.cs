using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace SchoolSystem.Infrastructure.Data.Context;

// Used only by dotnet-ef CLI at design time (migrations).
// Uses SQL Server provider so generated migrations have correct column types
// (uniqueidentifier, nvarchar, datetime2, decimal, bit).
// No real SQL Server connection is needed — EF Core reads the model only.
public class SchoolDataContextFactory : IDesignTimeDbContextFactory<SchoolDataContext>
{
    public SchoolDataContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<SchoolDataContext>()
            .UseSqlServer("Server=localhost;Database=SchoolSystem;Trusted_Connection=True;")
            .Options;

        return new SchoolDataContext(options);
    }
}
