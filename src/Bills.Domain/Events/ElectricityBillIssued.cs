using Bills.Domain.Common;
using Bills.Domain.ValueObjects;

namespace Bills.Domain.Events;

/// <summary>
/// Raised when a draft electricity bill is issued.
/// </summary>
public sealed record ElectricityBillIssued(
    ElectricityBillId BillId,
    DateTimeOffset IssuedAt,
    Money TotalAmount) : IDomainEvent
{
    /// <inheritdoc />
    public DateTimeOffset OccurredOn { get; } = DateTimeOffset.UtcNow;
}
