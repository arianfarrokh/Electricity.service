using Bills.Domain.Common;
using Bills.Domain.Enums;
using Bills.Domain.Events;
using Bills.Domain.ValueObjects;

namespace Bills.Domain.Aggregates;

/// <summary>
/// Aggregate root for electricity bill issuance within the Bill Issuance bounded context.
/// </summary>
public sealed class ElectricityBill : IHasDomainEvents
{
    private readonly List<IDomainEvent> _domainEvents = [];

    private ElectricityBill()
    {
        Id = default;
        SubscriptionId = null!;
        BillingPeriod = null!;
    }

    private ElectricityBill(
        ElectricityBillId id,
        SubscriptionId subscriptionId,
        BillingPeriod billingPeriod,
        BillStatus status)
    {
        Id = id;
        SubscriptionId = subscriptionId;
        BillingPeriod = billingPeriod;
        Status = status;
    }

    /// <summary>
    /// Gets the bill identifier.
    /// </summary>
    public ElectricityBillId Id { get; private set; }

    /// <summary>
    /// Gets the customer subscription identifier.
    /// </summary>
    public SubscriptionId SubscriptionId { get; private set; } = null!;

    /// <summary>
    /// Gets the billing period.
    /// </summary>
    public BillingPeriod BillingPeriod { get; private set; } = null!;

    /// <summary>
    /// Gets the meter reading, if one has been recorded.
    /// </summary>
    public MeterReading? MeterReading { get; private set; }

    /// <summary>
    /// Gets the tariff rate applied to consumption, if set.
    /// </summary>
    public TariffRate? TariffRate { get; private set; }

    /// <summary>
    /// Gets the calculated total amount, if available.
    /// </summary>
    public Money? TotalAmount { get; private set; }

    /// <summary>
    /// Gets the current bill status.
    /// </summary>
    public BillStatus Status { get; private set; }

    /// <summary>
    /// Gets the UTC timestamp when the bill was issued, if applicable.
    /// </summary>
    public DateTimeOffset? IssuedAt { get; private set; }

    /// <inheritdoc />
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    /// <inheritdoc />
    public void ClearDomainEvents() => _domainEvents.Clear();

    /// <summary>
    /// Creates a new bill in draft status.
    /// </summary>
    /// <param name="subscriptionId">The customer subscription.</param>
    /// <param name="billingPeriod">The billing period.</param>
    /// <returns>A new draft electricity bill.</returns>
    public static ElectricityBill CreateDraft(SubscriptionId subscriptionId, BillingPeriod billingPeriod)
    {
        var bill = new ElectricityBill(
            ElectricityBillId.New(),
            subscriptionId,
            billingPeriod,
            BillStatus.Draft);

        bill.Raise(new ElectricityBillDraftCreated(bill.Id, subscriptionId));
        return bill;
    }

    /// <summary>
    /// Records meter readings and calculates the total bill amount.
    /// </summary>
    /// <param name="reading">The meter reading.</param>
    /// <param name="rate">The tariff rate per kWh.</param>
    /// <exception cref="DomainException">Thrown when the bill cannot be modified.</exception>
    public void AddMeterReading(MeterReading reading, TariffRate rate)
    {
        EnsureDraft("Meter readings can only be added to draft bills.");

        if (rate.Currency is null)
        {
            throw new DomainException("Tariff rate currency is required.");
        }

        MeterReading = reading;
        TariffRate = rate;
        TotalAmount = Money.Create(reading.ConsumptionKwh * rate.PricePerKwh, rate.Currency);
    }

    /// <summary>
    /// Issues the bill to the customer.
    /// </summary>
    /// <param name="issuedAt">The UTC issuance timestamp.</param>
    /// <exception cref="DomainException">Thrown when issuance preconditions are not met.</exception>
    public void Issue(DateTimeOffset issuedAt)
    {
        EnsureDraft("Only draft bills can be issued.");

        if (MeterReading is null || TotalAmount is null)
        {
            throw new DomainException("Bill must have meter reading and total amount before it can be issued.");
        }

        Status = BillStatus.Issued;
        IssuedAt = issuedAt;
        Raise(new ElectricityBillIssued(Id, issuedAt, TotalAmount));
    }

    private void EnsureDraft(string message)
    {
        if (Status != BillStatus.Draft)
        {
            throw new DomainException(message);
        }
    }

    private void Raise(IDomainEvent domainEvent) => _domainEvents.Add(domainEvent);
}
