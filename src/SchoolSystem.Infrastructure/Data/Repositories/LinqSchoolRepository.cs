using Microsoft.EntityFrameworkCore;
using SchoolSystem.Domain.Models.Entities;
using SchoolSystem.Domain.Models.ValueObjects;
using SchoolSystem.Infrastructure.Data.Context;
using SchoolSystem.Infrastructure.Data.Interfaces;
using SchoolSystem.Infrastructure.Data.LinqEntities;

namespace SchoolSystem.Infrastructure.Data.Repositories;

public class LinqSchoolRepository
    : LinqRepositoryBase<SchoolEntity, School>, ISchoolRepository
{
    public LinqSchoolRepository(SchoolDataContext context)
        : base(context, Guid.Empty) { }

    public async Task<School?> GetByIdAsync(Guid schoolId, CancellationToken ct = default) =>
        await FirstOrDefaultAsync(Table.Where(s => s.SchoolId == schoolId), ct);

    public async Task<IReadOnlyList<School>> GetAllAsync(CancellationToken ct = default) =>
        await ToListAsync(Table.Where(s => s.IsActive).OrderBy(s => s.Name), ct);

    public async Task AddAsync(School school, CancellationToken ct = default) =>
        await Table.AddAsync(MapToEntity(school), ct);

    public void Update(School school)
    {
        var entity = MapToEntity(school);
        _context.Update(entity);
    }

    public void Delete(School school) =>
        Table.Remove(MapToEntity(school));

    protected override School MapToDomain(SchoolEntity e) =>
        School.Reconstitute(
            e.SchoolId,
            e.Name,
            new Address(e.Street, e.City, e.State, e.ZipCode, e.Country),
            new ContactInfo(e.Email, e.Phone),
            e.LogoUrl,
            e.IsActive,
            e.CreatedAt);

    protected override SchoolEntity MapToEntity(School s) => new()
    {
        SchoolId  = s.SchoolId,
        Name      = s.Name,
        Street    = s.Address.Street,
        City      = s.Address.City,
        State     = s.Address.State,
        ZipCode   = s.Address.ZipCode,
        Country   = s.Address.Country,
        Email     = s.Contact.Email,
        Phone     = s.Contact.Phone,
        LogoUrl   = s.LogoUrl,
        IsActive  = s.IsActive,
        CreatedAt = s.CreatedAt
    };
}
