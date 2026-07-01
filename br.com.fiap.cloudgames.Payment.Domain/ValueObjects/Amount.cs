using br.com.fiap.cloudgames.Payment.Domain.Exceptions;

namespace br.com.fiap.cloudgames.Payment.Domain.ValueObjects;

public sealed record Amount
{
    public decimal Value { get; }
    public string Currency { get; }

    public Amount(decimal value, string currency)
    {
        var errors = new List<string>();

        if (value < 0)
            errors.Add("Amount cannot be negative.");

        if (string.IsNullOrWhiteSpace(currency))
            errors.Add("Currency is required.");

        if (!string.IsNullOrWhiteSpace(currency))
        {
            currency = currency.Trim().ToUpperInvariant();

            if (currency.Length != 3)
                errors.Add("Currency must be a valid ISO 4217 code.");
        }

        if (errors.Any())
            throw new DomainException(errors);

        Value = value;
        Currency = currency;
    }

    public override string ToString() => $"{Value:F2} {Currency}";
}