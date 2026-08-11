namespace Bills.Application.Abstractions;

/// <summary>
/// Persists pending changes made through repositories.
/// </summary>
public interface IUnitOfWork
{
    /// <summary>
    /// Saves all pending changes to the data store.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The number of affected records.</returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
