namespace SchoolSystem.Application.Features.Invoices.DTOs;

public record InvoiceItemDto(
    Guid    InvoiceItemId,
    Guid?   CourseId,
    string  Description,
    decimal Amount,
    string  Currency);

public record InvoiceDto(
    Guid                        InvoiceId,
    Guid                        SchoolId,
    Guid                        StudentUserId,
    Guid                        ParentUserId,
    Guid                        AcademicYearId,
    decimal                     TotalAmount,
    decimal                     PaidAmount,
    decimal                     OutstandingBalance,
    string                      Currency,
    string                      Status,
    DateOnly                    DueDate,
    DateTime                    IssuedDate,
    DateTime?                   PaidDate,
    string?                     Notes,
    IReadOnlyList<InvoiceItemDto> Items);
