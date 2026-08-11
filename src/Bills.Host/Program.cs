using Bills.Application;
using Bills.Application.Bills.CreateDraft;
using Bills.Application.Bills.GetById;
using Bills.Application.Bills.Issue;
using Bills.Infrastructure;
using MediatR;
using NodaTime;
using NodaTime.Text;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? "Data Source=bills.db";

builder.Services.AddApplication();
builder.Services.AddInfrastructure(connectionString);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new() { Title = "Electricity Bill DDD Learning API", Version = "v1" });
});
builder.Services.AddHealthChecks()
    .AddDbContextCheck<Bills.Infrastructure.Persistence.BillsDbContext>("database");

var app = builder.Build();

app.ApplyMigrations();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapHealthChecks("/health");

app.MapPost("/api/bills/draft", async (CreateDraftBillRequest request, ISender sender) =>
{
    var localDatePattern = LocalDatePattern.Iso;

    if (!localDatePattern.Parse(request.PeriodStart).TryGetValue(out var periodStart) ||
        !localDatePattern.Parse(request.PeriodEnd).TryGetValue(out var periodEnd))
    {
        return Results.BadRequest(new { error = "Invalid date format. Use ISO format yyyy-MM-dd." });
    }

    var command = new CreateDraftBillCommand(
        request.SubscriptionNumber,
        periodStart,
        periodEnd,
        request.PreviousKwh,
        request.CurrentKwh,
        request.PricePerKwh,
        request.Currency);

    var result = await sender.Send(command);

    return result.IsSuccess
        ? Results.Created($"/api/bills/{result.Value!.BillId}", result.Value)
        : Results.BadRequest(new { error = result.Error });
})
.WithName("CreateDraftBill")
.WithTags("Bills")
.WithOpenApi();

app.MapPost("/api/bills/{id:guid}/issue", async (Guid id, IssueBillRequest? request, ISender sender) =>
{
    var issuedAt = request?.IssuedAt ?? DateTimeOffset.UtcNow;
    var result = await sender.Send(new IssueBillCommand(id, issuedAt));

    return result.IsSuccess
        ? Results.Ok(result.Value)
        : Results.BadRequest(new { error = result.Error });
})
.WithName("IssueBill")
.WithTags("Bills")
.WithOpenApi();

app.MapGet("/api/bills/{id:guid}", async (Guid id, ISender sender) =>
{
    var result = await sender.Send(new GetBillByIdQuery(id));

    return result.IsSuccess
        ? Results.Ok(result.Value)
        : Results.NotFound(new { error = result.Error });
})
.WithName("GetBillById")
.WithTags("Bills")
.WithOpenApi();

app.Run();

/// <summary>
/// HTTP request body for creating a draft bill.
/// </summary>
public sealed record CreateDraftBillRequest(
    string SubscriptionNumber,
    string PeriodStart,
    string PeriodEnd,
    decimal PreviousKwh,
    decimal CurrentKwh,
    decimal PricePerKwh,
    string Currency);

/// <summary>
/// HTTP request body for issuing a bill.
/// </summary>
public sealed record IssueBillRequest(DateTimeOffset? IssuedAt);
