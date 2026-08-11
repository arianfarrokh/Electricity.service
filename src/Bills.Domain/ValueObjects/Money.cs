namespace Bills.Domain.ValueObjects;

/// <summary>
/// Represents a monetary amount with currency.
/// </summary>
public sealed record Money
{
    private Money(decimal amount, string currency)
    {
        Amount = amount;
        Currency = currency;
    }

    /// <summary>
    /// Gets the monetary amount.
    /// </summary>
    public decimal Amount { get; }

    /// <summary>
    /// Gets the currency code (ISO 4217).
    /// </summary>
    public string Currency { get; }

    /// <summary>
    /// Creates a validated money value.
    /// </summary>
    /// <param name="amount">The amount.</param>
    /// <param name="currency">The currency code.</param>
    /// <returns>A new <see cref="Money"/> instance.</returns>
    /// <exception cref="ArgumentException">Thrown when the amount or currency is invalid.</exception>
    public static Money Create(decimal amount, string currency)
    {
        if (!TryCreate(amount, currency, out var money))
        {
            throw new ArgumentException("Amount must be non-negative and currency is required.", nameof(amount));
        }

        return money!;
    }

    /// <summary>
    /// Attempts to create a money value.
    /// </summary>
    /// <param name="amount">The amount.</param>
    /// <param name="currency">The currency code.</param>
    /// <param name="money">The created value when successful.</param>
    /// <returns><c>true</c> if creation succeeded; otherwise, <c>false</c>.</returns>
    public static bool TryCreate(decimal amount, string? currency, out Money? money)
    {
        money = null;

        if (amount < 0 || string.IsNullOrWhiteSpace(currency))
        {
            return false;
        }

        money = new Money(amount, currency.Trim().ToUpperInvariant());
        return true;
    }

    /// <summary>
    /// Multiplies the amount by a consumption factor while preserving currency.
    /// </summary>
    /// <param name="factor">The multiplier (e.g. kWh consumed).</param>
    /// <returns>The resulting <see cref="Money"/> value.</returns>
    public Money Multiply(decimal factor)
    {
        return Create(Amount * factor, Currency);
    }
}
