namespace SchoolSystem.Infrastructure.Services;

public interface ICurrentSchoolContext
{
    Guid   SchoolId { get; }
    Guid   UserId   { get; }
    string Email    { get; }
    bool   IsAuthenticated { get; }
}
