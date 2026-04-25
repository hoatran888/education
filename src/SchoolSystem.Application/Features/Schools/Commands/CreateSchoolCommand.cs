using FluentValidation;
using MediatR;
using SchoolSystem.Application.Common.Interfaces;
using SchoolSystem.Domain.Models.Entities;
using SchoolSystem.Domain.Models.Enums;
using SchoolSystem.Domain.Models.ValueObjects;
using SchoolSystem.Infrastructure.Data.Interfaces;

namespace SchoolSystem.Application.Features.Schools.Commands;

[RequireRoles(UserRole.SuperAdmin)]
public record CreateSchoolCommand(
    string  Name,
    string  Street,
    string  City,
    string  State,
    string  ZipCode,
    string  Country,
    string  Email,
    string  Phone,
    string? LogoUrl = null) : ICommand<Guid>;

public class CreateSchoolCommandValidator : AbstractValidator<CreateSchoolCommand>
{
    public CreateSchoolCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Phone).NotEmpty();
        RuleFor(x => x.Street).NotEmpty();
        RuleFor(x => x.City).NotEmpty();
        RuleFor(x => x.Country).NotEmpty();
    }
}

public class CreateSchoolCommandHandler : IRequestHandler<CreateSchoolCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateSchoolCommandHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<Guid> Handle(CreateSchoolCommand request, CancellationToken ct)
    {
        var address = new Address(request.Street, request.City, request.State,
                                  request.ZipCode, request.Country);
        var contact = new ContactInfo(request.Email, request.Phone);
        var school  = School.Create(request.Name, address, contact, request.LogoUrl);
        await _unitOfWork.Schools.AddAsync(school, ct);
        return school.SchoolId;
    }
}
