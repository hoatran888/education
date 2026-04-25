using FluentValidation;
using MediatR;
using SchoolSystem.Application.Common.Exceptions;
using SchoolSystem.Application.Common.Interfaces;
using SchoolSystem.Domain.Models.Entities;
using SchoolSystem.Domain.Models.Enums;
using SchoolSystem.Domain.Models.ValueObjects;
using SchoolSystem.Infrastructure.Data.Interfaces;

namespace SchoolSystem.Application.Features.Schools.Commands;

[RequireRoles(UserRole.SuperAdmin)]
public record UpdateSchoolCommand(
    Guid   SchoolId,
    string Name,
    string Street,
    string City,
    string State,
    string ZipCode,
    string Country,
    string Email,
    string Phone) : ICommand;

public class UpdateSchoolCommandValidator : AbstractValidator<UpdateSchoolCommand>
{
    public UpdateSchoolCommandValidator()
    {
        RuleFor(x => x.SchoolId).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Phone).NotEmpty();
    }
}

public class UpdateSchoolCommandHandler : IRequestHandler<UpdateSchoolCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateSchoolCommandHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task Handle(UpdateSchoolCommand request, CancellationToken ct)
    {
        var school = await _unitOfWork.Schools.GetByIdAsync(request.SchoolId, ct)
            ?? throw new NotFoundException(nameof(School), request.SchoolId);

        var address = new Address(request.Street, request.City, request.State,
                                  request.ZipCode, request.Country);
        var contact = new ContactInfo(request.Email, request.Phone);
        school.UpdateInfo(request.Name, address, contact);
        _unitOfWork.Schools.Update(school);
    }
}
