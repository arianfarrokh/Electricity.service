namespace Bills.Domain.Common;

/// <summary>
/// Marks an aggregate root that collects domain events during state changes.
/// </summary>
public interface IHasDomainEvents
{
    /// <summary>
    /// Gets the domain events raised by this aggregate since it was loaded or last cleared.
    /// </summary>
    IReadOnlyCollection<IDomainEvent> DomainEvents { get; }

    /// <summary>
    /// Clears all pending domain events, typically after they have been dispatched.
    /// </summary>
    void ClearDomainEvents();
}
