using Microsoft.EntityFrameworkCore;
using SchoolSystem.Infrastructure.Data.Context;

namespace SchoolSystem.Infrastructure.Data.Repositories;

public abstract class LinqRepositoryBase<TEntity, TDomain>
    where TEntity : class
{
    protected readonly SchoolDataContext _context;
    protected readonly Guid              _schoolId;

    protected LinqRepositoryBase(SchoolDataContext context, Guid schoolId)
    {
        _context  = context;
        _schoolId = schoolId;
    }

    protected DbSet<TEntity> Table => _context.Set<TEntity>();

    protected async Task<List<TDomain>> ToListAsync(
        IQueryable<TEntity> query,
        CancellationToken ct = default) =>
        (await query.ToListAsync(ct)).Select(MapToDomain).ToList();

    protected async Task<TDomain?> FirstOrDefaultAsync(
        IQueryable<TEntity> query,
        CancellationToken ct = default)
    {
        var entity = await query.FirstOrDefaultAsync(ct);
        return entity is null ? default : MapToDomain(entity);
    }

    protected Task<bool> AnyAsync(
        IQueryable<TEntity> query,
        CancellationToken ct = default) =>
        query.AnyAsync(ct);

    protected Task<int> CountAsync(
        IQueryable<TEntity> query,
        CancellationToken ct = default) =>
        query.CountAsync(ct);

    protected abstract TDomain MapToDomain(TEntity entity);
    protected abstract TEntity MapToEntity(TDomain domain);
}
