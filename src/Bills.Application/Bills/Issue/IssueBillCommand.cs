using Bills.Application.Abstractions;
using Bills.Application.Common;
using Bills.Domain.ValueObjects;
using FluentValidation;
using MediatR;

namespace Bills.Application.Bills.Issue;

/// <summary>
/// Command to issue a draft electricity bill.
/// </summary>
public sealed record IssueBillCommand(Guid BillId, DateTimeOffset IssuedAt) : IRequest<ApplicationResult<IssueBillResult>>;

/// <summary>
/// Result returned after issuing a bill.
/// </summary>
public sealed record IssueBillResult(Guid BillId, string Status, DateTimeOffset IssuedAt, decimal TotalAmount, string Currency);

/// <summary>
/// Validates <see cref="IssueBillCommand"/> requests.
/// </summary>
public sealed class IssueBillCommandValidator : AbstractValidator<IssueBillCommand>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="IssueBillCommandValidator"/> class.
    /// </summary>
    public IssueBillCommandValidator()
    {
        RuleFor(x => x.BillId).NotEmpty();
        RuleFor(x => x.IssuedAt).NotEqual(default(DateTimeOffset));
    }
}

/// <summary>
/// Handles bill issuance.
/// </summary>
public sealed class IssueBillCommandHandler : IRequestHandler<IssueBillCommand, ApplicationResult<IssueBillResult>>
{
    private readonly IElectricityBillRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>
    /// Initializes a new instance of the <see cref="IssueBillCommandHandler"/> class.
    /// </summary>
    public IssueBillCommandHandler(IElectricityBillRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    /// <inheritdoc />
    public async Task<ApplicationResult<IssueBillResult>> Handle(
        IssueBillCommand request,
        CancellationToken cancellationToken)
    {
        var billId = ElectricityBillId.From(request.BillId);
        var bill = await _repository.FindByIdAsync(billId, cancellationToken);

        if (bill is null)
        {
            return ApplicationResult<IssueBillResult>.Failure($"Bill '{request.BillId}' was not found.");
        }

        try
        {
            bill.Issue(request.IssuedAt);
        }
        catch (Domain.Common.DomainException ex)
        {
            return ApplicationResult<IssueBillResult>.Failure(ex.Message);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ApplicationResult<IssueBillResult>.Success(
            new IssueBillResult(
                bill.Id.Value,
                bill.Status.ToString(),
                bill.IssuedAt!.Value,
                bill.TotalAmount!.Amount,
                bill.TotalAmount.Currency));
    }
}
