using Bills.Domain.Aggregates;
using Bills.Domain.ValueObjects;

namespace Bills.Application.Abstractions;

/// <summary>
/// Persistence port for electricity bill aggregates.
/// </summary>
public interface IElectricityBillRepository
{
    /// <summary>
    /// Finds a bill by its identifier.
    /// </summary>
    /// <param name="id">The bill identifier.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The bill if found; otherwise, <c>null</c>.</returns>
    Task<ElectricityBill?> FindByIdAsync(ElectricityBillId id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new bill aggregate to the store.
    /// </summary>
    /// <param name="bill">The bill to add.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    Task AddAsync(ElectricityBill bill, CancellationToken cancellationToken = default);
}
