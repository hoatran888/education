using FluentValidation;
using MediatR;
using SchoolSystem.Application.Common.Interfaces;
using SchoolSystem.Domain.Models.Entities;
using SchoolSystem.Domain.Models.Enums;
using SchoolSystem.Domain.Models.ValueObjects;
using SchoolSystem.Infrastructure.Data.Interfaces;
using SchoolSystem.Infrastructure.Services;

namespace SchoolSystem.Application.Features.Invoices.Commands;

public record InvoiceItemRequest(string Description, decimal Amount, Guid? CourseId = null);

[RequireRoles(UserRole.Admin, UserRole.HRFinancial, UserRole.SuperAdmin)]
public record CreateInvoiceCommand(
    Guid                        StudentUserId,
    Guid                        ParentUserId,
    Guid                        AcademicYearId,
    DateOnly                    DueDate,
    string                      Currency,
    IReadOnlyList<InvoiceItemRequest> Items) : ICommand<Guid>;

public class CreateInvoiceCommandValidator : AbstractValidator<CreateInvoiceCommand>
{
    public CreateInvoiceCommandValidator()
    {
        RuleFor(x => x.StudentUserId).NotEmpty();
        RuleFor(x => x.ParentUserId).NotEmpty();
        RuleFor(x => x.AcademicYearId).NotEmpty();
        RuleFor(x => x.Currency).NotEmpty().Length(3);
        RuleFor(x => x.Items).NotEmpty();
        RuleForEach(x => x.Items).ChildRules(item =>
        {
            item.RuleFor(i => i.Description).NotEmpty();
            item.RuleFor(i => i.Amount).GreaterThan(0);
        });
    }
}

public class CreateInvoiceCommandHandler : IRequestHandler<CreateInvoiceCommand, Guid>
{
    private readonly IUnitOfWork          _unitOfWork;
    private readonly ICurrentSchoolContext _context;

    public CreateInvoiceCommandHandler(IUnitOfWork unitOfWork, ICurrentSchoolContext context)
    {
        _unitOfWork = unitOfWork;
        _context    = context;
    }

    public async Task<Guid> Handle(CreateInvoiceCommand request, CancellationToken ct)
    {
        var invoice = Invoice.Create(
            _context.SchoolId, request.StudentUserId, request.ParentUserId,
            request.AcademicYearId, Money.Zero(request.Currency), request.DueDate);

        foreach (var item in request.Items)
        {
            var money       = new Money(item.Amount, request.Currency);
            var invoiceItem = InvoiceItem.Create(invoice.InvoiceId, item.Description, money, item.CourseId);
            invoice.AddItem(invoiceItem);
        }

        await _unitOfWork.Invoices.AddAsync(invoice, ct);
        return invoice.InvoiceId;
    }
}
