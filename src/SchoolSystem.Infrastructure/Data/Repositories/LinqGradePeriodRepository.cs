using SchoolSystem.Domain.Models.Entities;
using SchoolSystem.Domain.Models.Enums;
using SchoolSystem.Infrastructure.Data.Context;
using SchoolSystem.Infrastructure.Data.Interfaces;
using SchoolSystem.Infrastructure.Data.LinqEntities;

namespace SchoolSystem.Infrastructure.Data.Repositories;

public class LinqGradePeriodRepository
    : LinqRepositoryBase<GradePeriodEntity, GradePeriod>, IGradePeriodRepository
{
    public LinqGradePeriodRepository(SchoolDataContext context, Guid schoolId)
        : base(context, schoolId) { }

    public async Task<GradePeriod?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        await FirstOrDefaultAsync(
            Table.Where(gp => gp.SchoolId == _schoolId && gp.GradePeriodId == id), ct);

    protected override GradePeriod MapToDomain(GradePeriodEntity e) =>
        GradePeriod.Reconstitute(
            e.GradePeriodId,
            e.SchoolId,
            e.TermId,
            e.Name,
            (GradePeriodType)e.Type,
            DateOnly.FromDateTime(e.StartDate),
            DateOnly.FromDateTime(e.EndDate));

    protected override GradePeriodEntity MapToEntity(GradePeriod gp) => new()
    {
        GradePeriodId = gp.GradePeriodId,
        SchoolId      = gp.SchoolId,
        TermId        = gp.TermId,
        Name          = gp.Name,
        Type          = (int)gp.Type,
        StartDate     = gp.StartDate.ToDateTime(TimeOnly.MinValue),
        EndDate       = gp.EndDate.ToDateTime(TimeOnly.MinValue)
    };
}
