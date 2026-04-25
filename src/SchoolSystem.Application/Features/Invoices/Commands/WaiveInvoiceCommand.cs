using FluentValidation;
using MediatR;
using SchoolSystem.Application.Common.Exceptions;
using SchoolSystem.Application.Common.Interfaces;
using SchoolSystem.Domain.Models.Entities;
using SchoolSystem.Domain.Models.Enums;
using SchoolSystem.Infrastructure.Data.Interfaces;
using SchoolSystem.Infrastructure.Services;

namespace SchoolSystem.Application.Features.Invoices.Commands;

[RequireRoles(UserRole.Admin, UserRole.HRFinancial, UserRole.SuperAdmin)]
public record WaiveInvoiceCommand(Guid InvoiceId, string Reason) : ICommand;

public class WaiveInvoiceCommandValidator : AbstractValidator<WaiveInvoiceCommand>
{
    public WaiveInvoiceCommandValidator()
    {
        RuleFor(x => x.InvoiceId).NotEmpty();
        RuleFor(x => x.Reason).NotEmpty().MaximumLength(500);
    }
}

public class WaiveInvoiceCommandHandler : IRequestHandler<WaiveInvoiceCommand>
{
    private readonly IUnitOfWork          _unitOfWork;
    private readonly ICurrentSchoolContext _context;

    public WaiveInvoiceCommandHandler(IUnitOfWork unitOfWork, ICurrentSchoolContext context)
    {
        _unitOfWork = unitOfWork;
        _context    = context;
    }

    public async Task Handle(WaiveInvoiceCommand request, CancellationToken ct)
    {
        var invoice = await _unitOfWork.Invoices.GetByIdAsync(request.InvoiceId, ct)
            ?? throw new NotFoundException(nameof(Invoice), request.InvoiceId);

        invoice.Waive(request.Reason, _context.UserId);
        _unitOfWork.Invoices.Update(invoice);
    }
}
