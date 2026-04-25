using SchoolSystem.Domain.Models.Enums;

namespace SchoolSystem.Application.Common.Interfaces;

[AttributeUsage(AttributeTargets.Class)]
public sealed class RequireRolesAttribute : Attribute
{
    public UserRole[] Roles { get; }
    public RequireRolesAttribute(params UserRole[] roles) => Roles = roles;
}
