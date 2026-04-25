namespace SchoolSystem.Domain.Models.ValueObjects;

public record Address
{
    public string  Street  { get; init; }
    public string  City    { get; init; }
    public string  State   { get; init; }
    public string  ZipCode { get; init; }
    public string  Country { get; init; }

    public Address(
        string street,
        string city,
        string state,
        string zipCode,
        string country)
    {
        if (string.IsNullOrWhiteSpace(street))  throw new ArgumentException("Street is required.");
        if (string.IsNullOrWhiteSpace(city))    throw new ArgumentException("City is required.");
        if (string.IsNullOrWhiteSpace(country)) throw new ArgumentException("Country is required.");

        Street  = street.Trim();
        City    = city.Trim();
        State   = state?.Trim()   ?? string.Empty;
        ZipCode = zipCode?.Trim() ?? string.Empty;
        Country = country.Trim();
    }

    public override string ToString() =>
        $"{Street}, {City}, {State} {ZipCode}, {Country}";
}
