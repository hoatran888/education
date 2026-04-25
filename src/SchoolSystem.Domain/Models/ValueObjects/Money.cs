namespace SchoolSystem.Domain.Models.ValueObjects;

public record Money
{
    public decimal Amount   { get; init; }
    public string  Currency { get; init; }

    public Money(decimal amount, string currency)
    {
        if (amount < 0)
            throw new ArgumentException("Amount cannot be negative.", nameof(amount));
        if (string.IsNullOrWhiteSpace(currency))
            throw new ArgumentException("Currency is required.", nameof(currency));

        Amount   = Math.Round(amount, 2);
        Currency = currency.Trim().ToUpperInvariant();
    }

    public Money Add(Money other)
    {
        if (Currency != other.Currency)
            throw new InvalidOperationException(
                $"Cannot add {other.Currency} to {Currency}.");
        return new Money(Amount + other.Amount, Currency);
    }

    public Money Subtract(Money other)
    {
        if (Currency != other.Currency)
            throw new InvalidOperationException(
                $"Cannot subtract {other.Currency} from {Currency}.");
        return new Money(Amount - other.Amount, Currency);
    }

    public bool IsGreaterThan(Money other) =>
        Currency == other.Currency && Amount > other.Amount;

    public static Money Zero(string currency) => new(0, currency);

    public override string ToString() => $"{Amount:F2} {Currency}";
}
