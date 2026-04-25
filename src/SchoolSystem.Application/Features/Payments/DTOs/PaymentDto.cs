namespace SchoolSystem.Application.Features.Payments.DTOs;

public record PaymentDto(
    Guid     PaymentId,
    Guid     SchoolId,
    Guid     InvoiceId,
    Guid     ParentUserId,
    decimal  Amount,
    string   Currency,
    DateTime PaymentDate,
    string   Method,
    string   Reference,
    string   Status,
    string?  Notes,
    Guid     RecordedBy);
