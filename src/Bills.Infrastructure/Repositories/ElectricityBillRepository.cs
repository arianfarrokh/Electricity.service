using Bills.Application.Abstractions;
using Bills.Domain.Aggregates;
using Bills.Domain.ValueObjects;
using Bills.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Bills.Infrastructure.Repositories;

/// <summary>
/// EF Core implementation of <see cref="IElectricityBillRepository"/>.
/// </summary>
public sealed class ElectricityBillRepository : IElectricityBillRepository
{
    private readonly BillsDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="ElectricityBillRepository"/> class.
    /// </summary>
    /// <param name="context">The database context.</param>
    public ElectricityBillRepository(BillsDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public async Task AddAsync(ElectricityBill bill, CancellationToken cancellationToken = default)
    {
        await _context.ElectricityBills.AddAsync(bill, cancellationToken);
    }

    /// <inheritdoc />
    public Task<ElectricityBill?> FindByIdAsync(ElectricityBillId id, CancellationToken cancellationToken = default)
    {
        return _context.ElectricityBills.FirstOrDefaultAsync(b => b.Id == id, cancellationToken);
    }
}
