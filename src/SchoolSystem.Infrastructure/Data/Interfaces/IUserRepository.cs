using SchoolSystem.Domain.Models.Entities;
using SchoolSystem.Domain.Models.Enums;

namespace SchoolSystem.Infrastructure.Data.Interfaces;

public interface IUserRepository
{
    Task<User?>              GetByIdAsync(Guid userId, CancellationToken ct = default);
    Task<User?>              GetByB2CObjectIdAsync(string b2cObjectId, CancellationToken ct = default);
    Task<User?>              GetByEmailAsync(string email, CancellationToken ct = default);
    Task<IReadOnlyList<User>> GetByRoleAsync(UserRole role, CancellationToken ct = default);
    Task<IReadOnlyList<User>> GetAllAsync(CancellationToken ct = default);
    Task                     AddAsync(User user, CancellationToken ct = default);
    Task                     SetPasswordAsync(Guid userId, string passwordHash, CancellationToken ct = default);
    void                     Update(User user);
    void                     Delete(User user);
}
