using Bills.Application.Behaviors;
using Bills.Application.Bills.CreateDraft;
using Bills.Application.Bills.GetById;
using Bills.Application.Bills.Issue;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Bills.Application;

/// <summary>
/// Dependency injection extensions for the application layer.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Registers application services, MediatR handlers, validators, and pipeline behaviors.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The same service collection for chaining.</returns>
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = typeof(CreateDraftBillCommand).Assembly;

        services.AddMediatR(configuration =>
        {
            configuration.RegisterServicesFromAssembly(assembly);
            configuration.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });

        services.AddValidatorsFromAssembly(assembly);

        return services;
    }
}
