namespace SchoolSystem.Domain.Models.ValueObjects;

public record ContactInfo
{
    public string Email { get; init; }
    public string Phone { get; init; }

    public ContactInfo(string email, string phone)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email is required.");
        if (!email.Contains('@'))
            throw new ArgumentException("Invalid email format.");

        Email = email.Trim().ToLowerInvariant();
        Phone = phone?.Trim() ?? string.Empty;
    }

    public override string ToString() => $"{Email} | {Phone}";
}
