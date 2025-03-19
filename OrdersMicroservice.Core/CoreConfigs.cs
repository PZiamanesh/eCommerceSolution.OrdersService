using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OrdersMicroservice.Core.ServiceContracts;
using OrdersMicroservice.Core.Services;
using OrdersMicroService.Core.RabbitMQ;

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

        services.AddStackExchangeRedisCache(ops =>
        {
            ops.Configuration = $"{configuration["REDIS_HOST"]}:{configuration["REDIS_PORT"]}";
        });

        services.AddSingleton<IRabbitMQProductNameUpdateConsumer, RabbitMQProductNameUpdateConsumer>();

        services.AddHostedService<RabbitMQProductNameUpdateHostedService>(); // runs in the background which triggers consume queue in rabbitmq

        return services;
    }
}
