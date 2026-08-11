using Bills.Domain.Aggregates;
using Bills.Domain.Common;
using Bills.Domain.Enums;
using Bills.Domain.ValueObjects;
using NodaTime;

namespace Bills.Domain.Tests;

/// <summary>
/// Tests for the <see cref="ElectricityBill"/> aggregate root.
/// </summary>
public class ElectricityBillTests
{
    private static readonly SubscriptionId Subscription = SubscriptionId.Create("SUB-1001");
    private static readonly BillingPeriod Period = BillingPeriod.Create(
        new LocalDate(2026, 1, 1),
        new LocalDate(2026, 1, 31));
    private static readonly TariffRate Rate = TariffRate.Create(1500m, "IRR");

    /// <summary>
    /// Verifies that CreateDraft starts in Draft status.
    /// </summary>
    [Fact]
    public void CreateDraft_StartsInDraftStatus()
    {
        var bill = ElectricityBill.CreateDraft(Subscription, Period);

        Assert.Equal(BillStatus.Draft, bill.Status);
        Assert.Null(bill.MeterReading);
        Assert.Null(bill.TotalAmount);
    }

    /// <summary>
    /// Verifies that AddMeterReading calculates total correctly.
    /// </summary>
    [Fact]
    public void AddMeterReading_CalculatesTotalCorrectly()
    {
        var bill = ElectricityBill.CreateDraft(Subscription, Period);
        var reading = MeterReading.Create(1000m, 1250m);

        bill.AddMeterReading(reading, Rate);

        Assert.Equal(250m, bill.MeterReading!.ConsumptionKwh);
        Assert.Equal(375_000m, bill.TotalAmount!.Amount);
        Assert.Equal("IRR", bill.TotalAmount.Currency);
    }

    /// <summary>
    /// Verifies that Issue without reading throws.
    /// </summary>
    [Fact]
    public void Issue_WithoutReading_Throws()
    {
        var bill = ElectricityBill.CreateDraft(Subscription, Period);

        var exception = Assert.Throws<DomainException>(() => bill.Issue(DateTimeOffset.UtcNow));

        Assert.Contains("meter reading", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Verifies that Issue when Draft with reading succeeds.
    /// </summary>
    [Fact]
    public void Issue_WhenDraftWithReading_Succeeds()
    {
        var bill = ElectricityBill.CreateDraft(Subscription, Period);
        bill.AddMeterReading(MeterReading.Create(500m, 700m), Rate);
        var issuedAt = DateTimeOffset.UtcNow;

        bill.Issue(issuedAt);

        Assert.Equal(BillStatus.Issued, bill.Status);
        Assert.Equal(issuedAt, bill.IssuedAt);
    }

    /// <summary>
    /// Verifies that reading cannot be added after the bill is issued.
    /// </summary>
    [Fact]
    public void AddMeterReading_AfterIssued_Throws()
    {
        var bill = ElectricityBill.CreateDraft(Subscription, Period);
        bill.AddMeterReading(MeterReading.Create(500m, 700m), Rate);
        bill.Issue(DateTimeOffset.UtcNow);

        var exception = Assert.Throws<DomainException>(() =>
            bill.AddMeterReading(MeterReading.Create(700m, 800m), Rate));

        Assert.Contains("draft", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Verifies that negative kWh readings are rejected.
    /// </summary>
    [Fact]
    public void MeterReading_NegativeKwh_Rejected()
    {
        Assert.False(MeterReading.TryCreate(-10m, 100m, out _));
        Assert.False(MeterReading.TryCreate(100m, 50m, out _));

        var exception = Assert.Throws<ArgumentException>(() => MeterReading.Create(-10m, 100m));

        Assert.NotNull(exception.Message);
    }
}
