using FluentValidation;
using MediatR;
using SchoolSystem.Application.Common.Exceptions;
using SchoolSystem.Application.Common.Interfaces;
using SchoolSystem.Domain.Models.Entities;
using SchoolSystem.Domain.Models.Enums;
using SchoolSystem.Infrastructure.Data.Interfaces;

namespace SchoolSystem.Application.Features.Invoices.Commands;

[RequireRoles(UserRole.Admin, UserRole.HRFinancial, UserRole.SuperAdmin)]
public record SendInvoiceCommand(Guid InvoiceId) : ICommand;

public class SendInvoiceCommandValidator : AbstractValidator<SendInvoiceCommand>
{
    public SendInvoiceCommandValidator() => RuleFor(x => x.InvoiceId).NotEmpty();
}

public class SendInvoiceCommandHandler : IRequestHandler<SendInvoiceCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public SendInvoiceCommandHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task Handle(SendInvoiceCommand request, CancellationToken ct)
    {
        var invoice = await _unitOfWork.Invoices.GetByIdAsync(request.InvoiceId, ct)
            ?? throw new NotFoundException(nameof(Invoice), request.InvoiceId);

        invoice.Send();
        _unitOfWork.Invoices.Update(invoice);
    }
}
