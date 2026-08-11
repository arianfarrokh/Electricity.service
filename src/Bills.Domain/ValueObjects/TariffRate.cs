namespace Bills.Domain.ValueObjects;

/// <summary>
/// Represents the price charged per kilowatt-hour.
/// </summary>
public sealed record TariffRate
{
    private TariffRate(decimal pricePerKwh, string currency)
    {
        PricePerKwh = pricePerKwh;
        Currency = currency;
    }

    /// <summary>
    /// Gets the price per kWh.
    /// </summary>
    public decimal PricePerKwh { get; }

    /// <summary>
    /// Gets the currency code (ISO 4217).
    /// </summary>
    public string Currency { get; }

    /// <summary>
    /// Creates a validated tariff rate.
    /// </summary>
    /// <param name="pricePerKwh">The price per kWh.</param>
    /// <param name="currency">The currency code.</param>
    /// <returns>A new <see cref="TariffRate"/>.</returns>
    /// <exception cref="ArgumentException">Thrown when the rate is invalid.</exception>
    public static TariffRate Create(decimal pricePerKwh, string currency = "IRR")
    {
        if (!TryCreate(pricePerKwh, currency, out var rate))
        {
            throw new ArgumentException("Tariff rate must be non-negative and currency is required.", nameof(pricePerKwh));
        }

        return rate!;
    }

    /// <summary>
    /// Attempts to create a tariff rate.
    /// </summary>
    /// <param name="pricePerKwh">The price per kWh.</param>
    /// <param name="currency">The currency code.</param>
    /// <param name="rate">The created rate when successful.</param>
    /// <returns><c>true</c> if creation succeeded; otherwise, <c>false</c>.</returns>
    public static bool TryCreate(decimal pricePerKwh, string? currency, out TariffRate? rate)
    {
        rate = null;

        if (pricePerKwh < 0 || string.IsNullOrWhiteSpace(currency))
        {
            return false;
        }

        rate = new TariffRate(pricePerKwh, currency.Trim().ToUpperInvariant());
        return true;
    }
}
