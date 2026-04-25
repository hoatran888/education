using SchoolSystem.Domain.Models.Entities;

namespace SchoolSystem.Infrastructure.Data.Interfaces;

public interface IGradePeriodRepository
{
    Task<GradePeriod?> GetByIdAsync(Guid id, CancellationToken ct = default);
}
