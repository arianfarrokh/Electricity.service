using Bills.Domain.Aggregates;
using Bills.Domain.Enums;
using Bills.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NodaTime;

namespace Bills.Infrastructure.Persistence;

/// <summary>
/// EF Core database context for the Bill Issuance bounded context.
/// </summary>
public sealed class BillsDbContext : DbContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BillsDbContext"/> class.
    /// </summary>
    /// <param name="options">Context configuration options.</param>
    public BillsDbContext(DbContextOptions<BillsDbContext> options)
        : base(options)
    {
    }

    /// <summary>
    /// Gets the electricity bills set.
    /// </summary>
    public DbSet<ElectricityBill> ElectricityBills => Set<ElectricityBill>();

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new ElectricityBillConfiguration());
        base.OnModelCreating(modelBuilder);
    }
}

internal sealed class ElectricityBillConfiguration : IEntityTypeConfiguration<ElectricityBill>
{
    public void Configure(EntityTypeBuilder<ElectricityBill> builder)
    {
        builder.ToTable("electricity_bills");

        builder.HasKey(b => b.Id);

        builder.Property(b => b.Id)
            .HasColumnName("id")
            .HasConversion(
                id => id.Value,
                value => ElectricityBillId.From(value));

        builder.Property(b => b.Status)
            .HasColumnName("status")
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(b => b.IssuedAt)
            .HasColumnName("issued_at");

        builder.OwnsOne(b => b.SubscriptionId, subscription =>
        {
            subscription.Property(s => s.Value)
                .HasColumnName("subscription_number")
                .HasMaxLength(50)
                .IsRequired();
        });

        builder.OwnsOne(b => b.BillingPeriod, period =>
        {
            period.Property(p => p.Start)
                .HasColumnName("period_start")
                .HasConversion(
                    date => date.ToDateTimeUnspecified(),
                    value => LocalDate.FromDateTime(value))
                .IsRequired();

            period.Property(p => p.End)
                .HasColumnName("period_end")
                .HasConversion(
                    date => date.ToDateTimeUnspecified(),
                    value => LocalDate.FromDateTime(value))
                .IsRequired();
        });

        builder.OwnsOne(b => b.MeterReading, reading =>
        {
            reading.Property(r => r.PreviousKwh).HasColumnName("previous_kwh").HasPrecision(18, 4);
            reading.Property(r => r.CurrentKwh).HasColumnName("current_kwh").HasPrecision(18, 4);
            reading.Property(r => r.ConsumptionKwh).HasColumnName("consumption_kwh").HasPrecision(18, 4);
        });

        builder.OwnsOne(b => b.TariffRate, rate =>
        {
            rate.Property(r => r.PricePerKwh).HasColumnName("price_per_kwh").HasPrecision(18, 4);
            rate.Property(r => r.Currency).HasColumnName("tariff_currency").HasMaxLength(3);
        });

        builder.OwnsOne(b => b.TotalAmount, money =>
        {
            money.Property(m => m.Amount).HasColumnName("total_amount").HasPrecision(18, 2);
            money.Property(m => m.Currency).HasColumnName("total_currency").HasMaxLength(3);
        });

        builder.Ignore(b => b.DomainEvents);
    }
}
