namespace Bills.Domain.ValueObjects;

/// <summary>
/// Represents meter readings used to calculate electricity consumption.
/// </summary>
public sealed record MeterReading
{
    private MeterReading(decimal previousKwh, decimal currentKwh, decimal consumptionKwh)
    {
        PreviousKwh = previousKwh;
        CurrentKwh = currentKwh;
        ConsumptionKwh = consumptionKwh;
    }

    /// <summary>
    /// Gets the previous meter reading in kWh.
    /// </summary>
    public decimal PreviousKwh { get; }

    /// <summary>
    /// Gets the current meter reading in kWh.
    /// </summary>
    public decimal CurrentKwh { get; }

    /// <summary>
    /// Gets the calculated consumption in kWh.
    /// </summary>
    public decimal ConsumptionKwh { get; }

    /// <summary>
    /// Creates a validated meter reading.
    /// </summary>
    /// <param name="previousKwh">The previous reading in kWh.</param>
    /// <param name="currentKwh">The current reading in kWh.</param>
    /// <returns>A new <see cref="MeterReading"/>.</returns>
    /// <exception cref="ArgumentException">Thrown when readings are invalid.</exception>
    public static MeterReading Create(decimal previousKwh, decimal currentKwh)
    {
        if (!TryCreate(previousKwh, currentKwh, out var reading))
        {
            throw new ArgumentException("Current reading must be greater than or equal to previous reading and both must be non-negative.", nameof(currentKwh));
        }

        return reading!;
    }

    /// <summary>
    /// Attempts to create a meter reading.
    /// </summary>
    /// <param name="previousKwh">The previous reading in kWh.</param>
    /// <param name="currentKwh">The current reading in kWh.</param>
    /// <param name="reading">The created reading when successful.</param>
    /// <returns><c>true</c> if creation succeeded; otherwise, <c>false</c>.</returns>
    public static bool TryCreate(decimal previousKwh, decimal currentKwh, out MeterReading? reading)
    {
        reading = null;

        if (previousKwh < 0 || currentKwh < 0 || currentKwh < previousKwh)
        {
            return false;
        }

        reading = new MeterReading(previousKwh, currentKwh, currentKwh - previousKwh);
        return true;
    }
}
