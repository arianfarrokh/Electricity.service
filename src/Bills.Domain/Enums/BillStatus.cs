namespace Bills.Domain.Enums;

/// <summary>
/// Lifecycle status of an electricity bill.
/// </summary>
public enum BillStatus
{
    /// <summary>
    /// Bill is being prepared and can still be modified.
    /// </summary>
    Draft = 0,

    /// <summary>
    /// Bill has been issued to the customer.
    /// </summary>
    Issued = 1,

    /// <summary>
    /// Bill has been paid in full.
    /// </summary>
    Paid = 2,

    /// <summary>
    /// Bill was cancelled and is no longer valid.
    /// </summary>
    Cancelled = 3
}
