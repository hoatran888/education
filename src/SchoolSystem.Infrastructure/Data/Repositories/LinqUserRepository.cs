using Microsoft.EntityFrameworkCore;
using SchoolSystem.Domain.Models.Entities;
using SchoolSystem.Domain.Models.Enums;
using SchoolSystem.Domain.Models.ValueObjects;
using SchoolSystem.Infrastructure.Data.Context;
using SchoolSystem.Infrastructure.Data.Interfaces;
using SchoolSystem.Infrastructure.Data.LinqEntities;

namespace SchoolSystem.Infrastructure.Data.Repositories;

public class LinqUserRepository
    : LinqRepositoryBase<UserEntity, User>, IUserRepository
{
    public LinqUserRepository(SchoolDataContext context, Guid schoolId)
        : base(context, schoolId) { }

    public async Task<User?> GetByIdAsync(Guid userId, CancellationToken ct = default) =>
        await FirstOrDefaultAsync(
            Table.Include(u => u.UserRoles)
                 .Where(u => u.SchoolId == _schoolId && u.UserId == userId), ct);

    public async Task<User?> GetByB2CObjectIdAsync(string b2cObjectId, CancellationToken ct = default) =>
        await FirstOrDefaultAsync(
            Table.Include(u => u.UserRoles)
                 .Where(u => u.SchoolId == _schoolId && u.B2CObjectId == b2cObjectId), ct);

    public async Task<User?> GetByEmailAsync(string email, CancellationToken ct = default) =>
        await FirstOrDefaultAsync(
            Table.Include(u => u.UserRoles)
                 .Where(u => u.SchoolId == _schoolId && u.Email == email.ToLowerInvariant()), ct);

    public async Task<IReadOnlyList<User>> GetByRoleAsync(UserRole role, CancellationToken ct = default)
    {
        var roleInt = (int)role;
        var userIds = await _context.UserRoles
            .Where(r => r.SchoolId == _schoolId && r.Role == roleInt)
            .Select(r => r.UserId)
            .ToListAsync(ct);

        return await ToListAsync(
            Table.Include(u => u.UserRoles)
                 .Where(u => u.SchoolId == _schoolId && userIds.Contains(u.UserId))
                 .OrderBy(u => u.LastName).ThenBy(u => u.FirstName), ct);
    }

    public async Task<IReadOnlyList<User>> GetAllAsync(CancellationToken ct = default) =>
        await ToListAsync(
            Table.Include(u => u.UserRoles)
                 .Where(u => u.SchoolId == _schoolId && u.IsActive)
                 .OrderBy(u => u.LastName).ThenBy(u => u.FirstName), ct);

    public async Task AddAsync(User user, CancellationToken ct = default)
    {
        await Table.AddAsync(MapToEntity(user), ct);
        foreach (var role in user.Roles)
        {
            await _context.UserRoles.AddAsync(new UserRoleEntity
            {
                UserRoleId = Guid.NewGuid(),
                UserId     = user.UserId,
                SchoolId   = user.SchoolId,
                Role       = (int)role
            }, ct);
        }
    }

    public void Update(User user)
    {
        _context.Update(MapToEntity(user));

        var existing = _context.UserRoles
            .Where(r => r.SchoolId == _schoolId && r.UserId == user.UserId)
            .ToList();
        _context.UserRoles.RemoveRange(existing);

        foreach (var role in user.Roles)
        {
            _context.UserRoles.Add(new UserRoleEntity
            {
                UserRoleId = Guid.NewGuid(),
                UserId     = user.UserId,
                SchoolId   = user.SchoolId,
                Role       = (int)role
            });
        }
    }

    public void Delete(User user)
    {
        var roles = _context.UserRoles
            .Where(r => r.SchoolId == _schoolId && r.UserId == user.UserId)
            .ToList();
        _context.UserRoles.RemoveRange(roles);
        Table.Remove(MapToEntity(user));
    }

    protected override User MapToDomain(UserEntity e)
    {
        var roles = e.UserRoles.Select(r => (UserRole)r.Role).ToList();
        return User.Reconstitute(
            e.UserId,
            e.SchoolId,
            e.B2CObjectId,
            e.FirstName,
            e.LastName,
            (Sex)e.Sex,
            e.Email,
            e.Phone,
            new Address(e.Street, e.City, e.State, e.ZipCode, e.Country),
            e.PhotoUrl,
            e.IsActive,
            e.CreatedAt,
            roles);
    }

    protected override UserEntity MapToEntity(User u) => new()
    {
        UserId      = u.UserId,
        SchoolId    = u.SchoolId,
        B2CObjectId = u.B2CObjectId,
        FirstName   = u.FirstName,
        LastName    = u.LastName,
        Sex         = (int)u.Sex,
        Email       = u.Email,
        Phone       = u.Phone,
        Street      = u.Address.Street,
        City        = u.Address.City,
        State       = u.Address.State,
        ZipCode     = u.Address.ZipCode,
        Country     = u.Address.Country,
        PhotoUrl    = u.PhotoUrl,
        IsActive    = u.IsActive,
        CreatedAt   = u.CreatedAt
    };
}
