using Microsoft.AspNetCore.Identity;

namespace SchoolSystem.Infrastructure.Services;

public class AspNetPasswordHasher : IPasswordHasher
{
    private readonly PasswordHasher<string> _inner = new();

    public string Hash(string password) =>
        _inner.HashPassword("", password);

    public bool Verify(string hash, string password) =>
        _inner.VerifyHashedPassword("", hash, password) != PasswordVerificationResult.Failed;
}
