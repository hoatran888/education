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
    string   FirstName,
    string   LastName,
    Sex      Sex,
    string   Email,
    string   Password,
    UserRole Role,
    string   Phone   = "",
    string   Street  = "",
    string   City    = "",
    string   State   = "",
    string   ZipCode = "",
    string   Country = "") : ICommand<Guid>;

public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty().MinimumLength(6);
        RuleFor(x => x.Role).IsInEnum();
    }
}

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Guid>
{
    private readonly IUnitOfWork           _unitOfWork;
    private readonly ICurrentSchoolContext _context;
    private readonly IPasswordHasher       _hasher;

    public CreateUserCommandHandler(
        IUnitOfWork unitOfWork, ICurrentSchoolContext context, IPasswordHasher hasher)
    {
        _unitOfWork = unitOfWork;
        _context    = context;
        _hasher     = hasher;
    }

    public async Task<Guid> Handle(CreateUserCommand request, CancellationToken ct)
    {
        var address = new Address(
            string.IsNullOrWhiteSpace(request.Street)  ? "N/A" : request.Street,
            string.IsNullOrWhiteSpace(request.City)    ? "N/A" : request.City,
            request.State,
            request.ZipCode,
            string.IsNullOrWhiteSpace(request.Country) ? "N/A" : request.Country);

        var user = User.Create(
            _context.SchoolId,
            Guid.NewGuid().ToString(),   // B2CObjectId placeholder for local auth
            request.FirstName, request.LastName,
            request.Sex, request.Email.ToLowerInvariant(),
            request.Phone, address);

        user.AssignRole(request.Role);
        await _unitOfWork.Users.AddAsync(user, ct);
        await _unitOfWork.Users.SetPasswordAsync(user.UserId, _hasher.Hash(request.Password), ct);
        return user.UserId;
    }
}
