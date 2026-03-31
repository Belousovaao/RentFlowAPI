using System;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using RentFlow.Application.Common.Behaviors;
using FluentValidation;

namespace RentFlow.Application;

public static class DependencyInjections
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(DependencyInjections).Assembly));
        services.AddValidatorsFromAssembly(typeof(DependencyInjections).Assembly);
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
        return services;
    }
}
