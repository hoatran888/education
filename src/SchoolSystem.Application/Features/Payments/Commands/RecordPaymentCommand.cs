using FluentValidation;
using MediatR;
using SchoolSystem.Application.Common.Exceptions;
using SchoolSystem.Application.Common.Interfaces;
using SchoolSystem.Domain.Models.Entities;
using SchoolSystem.Domain.Models.Enums;
using SchoolSystem.Domain.Models.ValueObjects;
using SchoolSystem.Infrastructure.Data.Interfaces;
using SchoolSystem.Infrastructure.Services;

namespace SchoolSystem.Application.Features.Payments.Commands;

[RequireRoles(UserRole.Admin, UserRole.HRFinancial, UserRole.SuperAdmin)]
public record RecordPaymentCommand(
    Guid          InvoiceId,
    Guid          ParentUserId,
    decimal       Amount,
    string        Currency,
    PaymentMethod Method,
    string        Reference,
    string?       Notes = null) : ICommand<Guid>;

public class RecordPaymentCommandValidator : AbstractValidator<RecordPaymentCommand>
{
    public RecordPaymentCommandValidator()
    {
        RuleFor(x => x.InvoiceId).NotEmpty();
        RuleFor(x => x.ParentUserId).NotEmpty();
        RuleFor(x => x.Amount).GreaterThan(0);
        RuleFor(x => x.Currency).NotEmpty().Length(3);
        RuleFor(x => x.Method).IsInEnum();
        RuleFor(x => x.Reference).NotEmpty();
    }
}

public class RecordPaymentCommandHandler : IRequestHandler<RecordPaymentCommand, Guid>
{
    private readonly IUnitOfWork          _unitOfWork;
    private readonly ICurrentSchoolContext _context;

    public RecordPaymentCommandHandler(IUnitOfWork unitOfWork, ICurrentSchoolContext context)
    {
        _unitOfWork = unitOfWork;
        _context    = context;
    }

    public async Task<Guid> Handle(RecordPaymentCommand request, CancellationToken ct)
    {
        var invoice = await _unitOfWork.Invoices.GetByIdAsync(request.InvoiceId, ct)
            ?? throw new NotFoundException(nameof(Invoice), request.InvoiceId);

        var amount  = new Money(request.Amount, request.Currency);
        var payment = Payment.Create(_context.SchoolId, request.InvoiceId, request.ParentUserId,
                                     amount, request.Method, request.Reference,
                                     _context.UserId, request.Notes);

        invoice.ApplyPayment(payment);

        await _unitOfWork.Payments.AddAsync(payment, ct);
        _unitOfWork.Invoices.Update(invoice);

        return payment.PaymentId;
    }
}
