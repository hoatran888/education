using FluentValidation;
using MediatR;
using SchoolSystem.Application.Common.Exceptions;
using SchoolSystem.Application.Common.Interfaces;
using SchoolSystem.Domain.Models.Entities;
using SchoolSystem.Domain.Models.Enums;
using SchoolSystem.Infrastructure.Data.Interfaces;

namespace SchoolSystem.Application.Features.Payments.Commands;

[RequireRoles(UserRole.Admin, UserRole.HRFinancial, UserRole.SuperAdmin)]
public record ConfirmPaymentCommand(Guid PaymentId) : ICommand;

public class ConfirmPaymentCommandValidator : AbstractValidator<ConfirmPaymentCommand>
{
    public ConfirmPaymentCommandValidator() => RuleFor(x => x.PaymentId).NotEmpty();
}

public class ConfirmPaymentCommandHandler : IRequestHandler<ConfirmPaymentCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public ConfirmPaymentCommandHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task Handle(ConfirmPaymentCommand request, CancellationToken ct)
    {
        var payment = await _unitOfWork.Payments.GetByIdAsync(request.PaymentId, ct)
            ?? throw new NotFoundException(nameof(Payment), request.PaymentId);

        payment.Confirm();
        _unitOfWork.Payments.Update(payment);
    }
}
