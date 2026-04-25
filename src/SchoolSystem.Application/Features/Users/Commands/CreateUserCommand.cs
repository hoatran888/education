using FluentValidation;
using MediatR;
using SchoolSystem.Application.Common.Interfaces;
using SchoolSystem.Domain.Models.Entities;
using SchoolSystem.Domain.Models.Enums;
using SchoolSystem.Domain.Models.ValueObjects;
using SchoolSystem.Infrastructure.Data.Interfaces;
using SchoolSystem.Infrastructure.Services;

namespace SchoolSystem.Application.Features.Users.Commands;

[RequireRoles(UserRole.Admin, UserRole.SuperAdmin)]
public record CreateUserCommand(
    string  FirstName,
    string  LastName,
    string  B2CObjectId,
    Sex     Sex,
    string  Email,
    string  Phone,
    string  Street,
    string  City,
    string  State,
    string  ZipCode,
    string  Country) : ICommand<Guid>;

public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.B2CObjectId).NotEmpty();
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Phone).NotEmpty();
    }
}

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Guid>
{
    private readonly IUnitOfWork          _unitOfWork;
    private readonly ICurrentSchoolContext _context;

    public CreateUserCommandHandler(IUnitOfWork unitOfWork, ICurrentSchoolContext context)
    {
        _unitOfWork = unitOfWork;
        _context    = context;
    }

    public async Task<Guid> Handle(CreateUserCommand request, CancellationToken ct)
    {
        var address = new Address(request.Street, request.City, request.State,
                                  request.ZipCode, request.Country);
        var user = User.Create(
            _context.SchoolId, request.B2CObjectId,
            request.FirstName, request.LastName,
            request.Sex, request.Email, request.Phone, address);
        await _unitOfWork.Users.AddAsync(user, ct);
        return user.UserId;
    }
}
