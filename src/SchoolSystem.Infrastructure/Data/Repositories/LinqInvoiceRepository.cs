using System.Reflection;
using Microsoft.EntityFrameworkCore;
using SchoolSystem.Domain.Models.Entities;
using SchoolSystem.Domain.Models.Enums;
using SchoolSystem.Domain.Models.ValueObjects;
using SchoolSystem.Infrastructure.Data.Context;
using SchoolSystem.Infrastructure.Data.Interfaces;
using SchoolSystem.Infrastructure.Data.LinqEntities;

namespace SchoolSystem.Infrastructure.Data.Repositories;

public class LinqInvoiceRepository
    : LinqRepositoryBase<InvoiceEntity, Invoice>, IInvoiceRepository
{
    public LinqInvoiceRepository(SchoolDataContext context, Guid schoolId)
        : base(context, schoolId) { }

    public async Task<Invoice?> GetByIdAsync(Guid invoiceId, CancellationToken ct = default)
    {
        var entity = await Table
            .Include(i => i.Items)
            .FirstOrDefaultAsync(i => i.SchoolId == _schoolId && i.InvoiceId == invoiceId, ct);
        return entity is null ? null : MapToDomainWithItems(entity);
    }

    public async Task<IReadOnlyList<Invoice>> GetByStudentAsync(
        Guid studentUserId, CancellationToken ct = default)
    {
        var entities = await Table
            .Include(i => i.Items)
            .Where(i => i.SchoolId == _schoolId && i.StudentUserId == studentUserId)
            .OrderByDescending(i => i.IssuedDate)
            .ToListAsync(ct);
        return entities.Select(MapToDomainWithItems).ToList();
    }

    public async Task<IReadOnlyList<Invoice>> GetByParentAsync(
        Guid parentUserId, CancellationToken ct = default)
    {
        var entities = await Table
            .Include(i => i.Items)
            .Where(i => i.SchoolId == _schoolId && i.ParentUserId == parentUserId)
            .OrderByDescending(i => i.IssuedDate)
            .ToListAsync(ct);
        return entities.Select(MapToDomainWithItems).ToList();
    }

    public async Task<IReadOnlyList<Invoice>> GetByStatusAsync(
        InvoiceStatus status, CancellationToken ct = default)
    {
        var statusStr = status.ToString();
        var entities = await Table
            .Include(i => i.Items)
            .Where(i => i.SchoolId == _schoolId && i.Status == statusStr)
            .OrderBy(i => i.DueDate)
            .ToListAsync(ct);
        return entities.Select(MapToDomainWithItems).ToList();
    }

    public async Task<IReadOnlyList<Invoice>> GetOverdueAsync(CancellationToken ct = default)
    {
        var overdueStr = nameof(InvoiceStatus.Overdue);
        var entities = await Table
            .Include(i => i.Items)
            .Where(i => i.SchoolId == _schoolId && i.Status == overdueStr)
            .OrderBy(i => i.DueDate)
            .ToListAsync(ct);
        return entities.Select(MapToDomainWithItems).ToList();
    }

    public async Task AddAsync(Invoice invoice, CancellationToken ct = default)
    {
        await Table.AddAsync(MapToEntity(invoice), ct);
        foreach (var item in invoice.Items)
        {
            await _context.InvoiceItems.AddAsync(new InvoiceItemEntity
            {
                InvoiceItemId = item.InvoiceItemId,
                InvoiceId     = item.InvoiceId,
                CourseId      = item.CourseId,
                Description   = item.Description,
                Amount        = item.Amount.Amount,
                Currency      = item.Amount.Currency
            }, ct);
        }
    }

    public void Update(Invoice invoice) =>
        _context.Update(MapToEntity(invoice));

    private Invoice MapToDomainWithItems(InvoiceEntity e)
    {
        var invoice = MapToDomain(e);
        var items = e.Items.Select(i => InvoiceItem.Create(
            i.InvoiceId,
            i.Description,
            new Money(i.Amount, i.Currency),
            i.CourseId)).ToList();

        var field = typeof(Invoice).GetField("_items",
            BindingFlags.NonPublic | BindingFlags.Instance)!;
        ((List<InvoiceItem>)field.GetValue(invoice)!).AddRange(items);
        return invoice;
    }

    protected override Invoice MapToDomain(InvoiceEntity e) =>
        Invoice.Reconstitute(
            e.InvoiceId,
            e.SchoolId,
            e.StudentUserId,
            e.ParentUserId,
            e.AcademicYearId,
            new Money(e.TotalAmount, e.Currency),
            new Money(e.PaidAmount,  e.Currency),
            Enum.Parse<InvoiceStatus>(e.Status),
            DateOnly.FromDateTime(e.DueDate),
            e.IssuedDate,
            e.PaidDate,
            e.Notes);

    protected override InvoiceEntity MapToEntity(Invoice i) => new()
    {
        InvoiceId      = i.InvoiceId,
        SchoolId       = i.SchoolId,
        StudentUserId  = i.StudentUserId,
        ParentUserId   = i.ParentUserId,
        AcademicYearId = i.AcademicYearId,
        TotalAmount    = i.TotalAmount.Amount,
        PaidAmount     = i.PaidAmount.Amount,
        Currency       = i.TotalAmount.Currency,
        Status         = i.Status.ToString(),
        DueDate        = i.DueDate.ToDateTime(TimeOnly.MinValue),
        IssuedDate     = i.IssuedDate,
        PaidDate       = i.PaidDate,
        Notes          = i.Notes
    };
}
