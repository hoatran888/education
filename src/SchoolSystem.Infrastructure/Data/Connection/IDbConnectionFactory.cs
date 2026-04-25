using System.Data;

namespace SchoolSystem.Infrastructure.Data.Connection;

public interface IDbConnectionFactory
{
    IDbConnection Create();
    Task<IDbConnection> CreateAsync(CancellationToken ct = default);
}
