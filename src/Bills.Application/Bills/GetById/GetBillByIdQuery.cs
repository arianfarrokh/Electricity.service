using Bills.Application.Abstractions;
using Bills.Application.Common;
using Bills.Domain.ValueObjects;
using MediatR;

namespace Bills.Application.Bills.GetById;

/// <summary>
/// Query to retrieve an electricity bill by identifier.
/// </summary>
public sealed record GetBillByIdQuery(Guid BillId) : IRequest<ApplicationResult<BillDetailsDto>>;

/// <summary>
/// Read model for bill details.
/// </summary>
public sealed record BillDetailsDto(
    Guid Id,
    string SubscriptionNumber,
    string PeriodStart,
    string PeriodEnd,
    string Status,
    decimal? PreviousKwh,
    decimal? CurrentKwh,
    decimal? ConsumptionKwh,
    decimal? PricePerKwh,
    decimal? TotalAmount,
    string? Currency,
    DateTimeOffset? IssuedAt);

/// <summary>
/// Handles bill lookup by identifier.
/// </summary>
public sealed class GetBillByIdQueryHandler : IRequestHandler<GetBillByIdQuery, ApplicationResult<BillDetailsDto>>
{
    private readonly IElectricityBillRepository _repository;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetBillByIdQueryHandler"/> class.
    /// </summary>
    public GetBillByIdQueryHandler(IElectricityBillRepository repository)
    {
        _repository = repository;
    }

    /// <inheritdoc />
    public async Task<ApplicationResult<BillDetailsDto>> Handle(
        GetBillByIdQuery request,
        CancellationToken cancellationToken)
    {
        var bill = await _repository.FindByIdAsync(ElectricityBillId.From(request.BillId), cancellationToken);

        if (bill is null)
        {
            return ApplicationResult<BillDetailsDto>.Failure($"Bill '{request.BillId}' was not found.");
        }

        var dto = new BillDetailsDto(
            bill.Id.Value,
            bill.SubscriptionId.Value,
            bill.BillingPeriod.Start.ToString(),
            bill.BillingPeriod.End.ToString(),
            bill.Status.ToString(),
            bill.MeterReading?.PreviousKwh,
            bill.MeterReading?.CurrentKwh,
            bill.MeterReading?.ConsumptionKwh,
            bill.TariffRate?.PricePerKwh,
            bill.TotalAmount?.Amount,
            bill.TotalAmount?.Currency,
            bill.IssuedAt);

        return ApplicationResult<BillDetailsDto>.Success(dto);
    }
}
