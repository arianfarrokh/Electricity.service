using Bills.Domain.Common;
using Bills.Domain.ValueObjects;

namespace Bills.Domain.Events;

/// <summary>
/// Raised when a new electricity bill draft is created.
/// </summary>
public sealed record ElectricityBillDraftCreated(
    ElectricityBillId BillId,
    SubscriptionId SubscriptionId) : IDomainEvent
{
    /// <inheritdoc />
    public DateTimeOffset OccurredOn { get; } = DateTimeOffset.UtcNow;
}
