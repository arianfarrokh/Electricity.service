using Bills.Application.Abstractions;
using Bills.Application.Common;
using Bills.Domain.Aggregates;
using Bills.Domain.ValueObjects;
using FluentValidation;
using MediatR;
using NodaTime;

namespace Bills.Application.Bills.CreateDraft;

/// <summary>
/// Command to create a new draft electricity bill.
/// </summary>
public sealed record CreateDraftBillCommand(
    string SubscriptionNumber,
    LocalDate PeriodStart,
    LocalDate PeriodEnd,
    decimal PreviousKwh,
    decimal CurrentKwh,
    decimal PricePerKwh,
    string Currency) : IRequest<ApplicationResult<CreateDraftBillResult>>;

/// <summary>
/// Result returned after creating a draft bill.
/// </summary>
public sealed record CreateDraftBillResult(Guid BillId, string Status);

/// <summary>
/// Validates <see cref="CreateDraftBillCommand"/> requests.
/// </summary>
public sealed class CreateDraftBillCommandValidator : AbstractValidator<CreateDraftBillCommand>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CreateDraftBillCommandValidator"/> class.
    /// </summary>
    public CreateDraftBillCommandValidator()
    {
        RuleFor(x => x.SubscriptionNumber).NotEmpty().MaximumLength(50);
        RuleFor(x => x.PeriodEnd).GreaterThan(x => x.PeriodStart);
        RuleFor(x => x.PreviousKwh).GreaterThanOrEqualTo(0);
        RuleFor(x => x.CurrentKwh).GreaterThanOrEqualTo(x => x.PreviousKwh);
        RuleFor(x => x.PricePerKwh).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Currency).NotEmpty().MaximumLength(3);
    }
}

/// <summary>
/// Handles creation of draft electricity bills.
/// </summary>
public sealed class CreateDraftBillCommandHandler : IRequestHandler<CreateDraftBillCommand, ApplicationResult<CreateDraftBillResult>>
{
    private readonly IElectricityBillRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateDraftBillCommandHandler"/> class.
    /// </summary>
    public CreateDraftBillCommandHandler(IElectricityBillRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    /// <inheritdoc />
    public async Task<ApplicationResult<CreateDraftBillResult>> Handle(
        CreateDraftBillCommand request,
        CancellationToken cancellationToken)
    {
        if (!SubscriptionId.TryCreate(request.SubscriptionNumber, out var subscriptionId) ||
            !BillingPeriod.TryCreate(request.PeriodStart, request.PeriodEnd, out var billingPeriod) ||
            !MeterReading.TryCreate(request.PreviousKwh, request.CurrentKwh, out var meterReading) ||
            !TariffRate.TryCreate(request.PricePerKwh, request.Currency, out var tariffRate))
        {
            return ApplicationResult<CreateDraftBillResult>.Failure("Invalid bill draft data.");
        }

        var bill = ElectricityBill.CreateDraft(subscriptionId!, billingPeriod!);
        bill.AddMeterReading(meterReading!, tariffRate!);

        await _repository.AddAsync(bill, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ApplicationResult<CreateDraftBillResult>.Success(
            new CreateDraftBillResult(bill.Id.Value, bill.Status.ToString()));
    }
}
