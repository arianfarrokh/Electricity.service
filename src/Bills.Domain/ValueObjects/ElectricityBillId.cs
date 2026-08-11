namespace Bills.Domain.ValueObjects;

/// <summary>
/// Strongly typed identifier for an electricity bill aggregate.
/// </summary>
public readonly record struct ElectricityBillId(Guid Value)
{
    /// <summary>
    /// Creates a new unique bill identifier.
    /// </summary>
    public static ElectricityBillId New() => new(Guid.NewGuid());

    /// <summary>
    /// Creates a bill identifier from an existing GUID value.
    /// </summary>
    /// <param name="value">The underlying GUID.</param>
    /// <returns>The strongly typed identifier.</returns>
    public static ElectricityBillId From(Guid value) => new(value);

    /// <inheritdoc />
    public override string ToString() => Value.ToString();
}
