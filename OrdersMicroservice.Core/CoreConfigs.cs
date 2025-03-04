using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OrdersMicroservice.Core.ServiceContracts;
using OrdersMicroservice.Core.Services;

namespace OrdersMicroservice.Core;

public static class CoreConfigs
{
    public static IServiceCollection AddCoreServices(
        this IServiceCollection services,
        IConfiguration configuration
        )
    {

        services.AddScoped<IOrderService, OrderService>();

        services.AddValidatorsFromAssemblyContaining(typeof(CoreConfigs));

        return services;
    }
}
