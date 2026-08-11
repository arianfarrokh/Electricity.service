using NodaTime;

namespace Bills.Domain.ValueObjects;

/// <summary>
/// Represents the billing period covered by an electricity bill.
/// </summary>
public sealed record BillingPeriod
{
    private BillingPeriod(LocalDate start, LocalDate end)
    {
        Start = start;
        End = end;
    }

    /// <summary>
    /// Gets the first day of the billing period (inclusive).
    /// </summary>
    public LocalDate Start { get; }

    /// <summary>
    /// Gets the last day of the billing period (inclusive).
    /// </summary>
    public LocalDate End { get; }

    /// <summary>
    /// Creates a validated billing period.
    /// </summary>
    /// <param name="start">The period start date.</param>
    /// <param name="end">The period end date.</param>
    /// <returns>A new <see cref="BillingPeriod"/>.</returns>
    /// <exception cref="ArgumentException">Thrown when the period is invalid.</exception>
    public static BillingPeriod Create(LocalDate start, LocalDate end)
    {
        if (!TryCreate(start, end, out var period))
        {
            throw new ArgumentException("Billing period end must be after start.", nameof(end));
        }

        return period!;
    }

    /// <summary>
    /// Attempts to create a billing period.
    /// </summary>
    /// <param name="start">The period start date.</param>
    /// <param name="end">The period end date.</param>
    /// <param name="period">The created period when successful.</param>
    /// <returns><c>true</c> if creation succeeded; otherwise, <c>false</c>.</returns>
    public static bool TryCreate(LocalDate start, LocalDate end, out BillingPeriod? period)
    {
        period = null;

        if (end <= start)
        {
            return false;
        }

        period = new BillingPeriod(start, end);
        return true;
    }
}
