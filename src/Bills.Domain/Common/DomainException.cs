namespace Bills.Domain.Common;

/// <summary>
/// Represents a violation of domain invariants or business rules.
/// </summary>
public sealed class DomainException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DomainException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    public DomainException(string message)
        : base(message)
    {
    }
}
