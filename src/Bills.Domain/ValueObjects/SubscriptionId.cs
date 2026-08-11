namespace Bills.Domain.ValueObjects;

/// <summary>
/// Represents a customer subscription number linked to an electricity bill.
/// </summary>
public sealed record SubscriptionId
{
    private SubscriptionId(string value)
    {
        Value = value;
    }

    /// <summary>
    /// Gets the subscription number.
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// Creates a validated subscription identifier.
    /// </summary>
    /// <param name="value">The subscription number.</param>
    /// <returns>A new <see cref="SubscriptionId"/>.</returns>
    /// <exception cref="ArgumentException">Thrown when the value is invalid.</exception>
    public static SubscriptionId Create(string value)
    {
        if (!TryCreate(value, out var subscriptionId))
        {
            throw new ArgumentException("Subscription number is required and cannot exceed 50 characters.", nameof(value));
        }

        return subscriptionId!;
    }

    /// <summary>
    /// Attempts to create a subscription identifier.
    /// </summary>
    /// <param name="value">The subscription number.</param>
    /// <param name="subscriptionId">The created identifier when successful.</param>
    /// <returns><c>true</c> if creation succeeded; otherwise, <c>false</c>.</returns>
    public static bool TryCreate(string? value, out SubscriptionId? subscriptionId)
    {
        subscriptionId = null;

        if (string.IsNullOrWhiteSpace(value) || value.Length > 50)
        {
            return false;
        }

        subscriptionId = new SubscriptionId(value.Trim());
        return true;
    }

    /// <inheritdoc />
    public override string ToString() => Value;
}
