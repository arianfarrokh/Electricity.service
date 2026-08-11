using Bills.Application.Abstractions;
using Bills.Infrastructure.Persistence;
using Bills.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Bills.Infrastructure;

/// <summary>
/// Dependency injection extensions for the infrastructure layer.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Registers infrastructure services including EF Core, repositories, and unit of work.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="connectionString">The SQLite connection string.</param>
    /// <returns>The same service collection for chaining.</returns>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<BillsDbContext>(options =>
            options.UseSqlite(connectionString));

        services.AddScoped<IElectricityBillRepository, ElectricityBillRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }

    /// <summary>
    /// Applies pending EF Core migrations to the database.
    /// </summary>
    /// <param name="services">The service provider.</param>
    public static void ApplyMigrations(this IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<BillsDbContext>();
        context.Database.Migrate();
    }
}
