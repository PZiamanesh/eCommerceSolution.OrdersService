using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using MongoDB.Driver.Core.Configuration;
using OrdersMicroservice.Core.RepositoryContracts;
using OrdersMicroservice.Infrastructure.Repositories;

namespace OrdersMicroservice.Infrastructure;

public static class InfraConfigs
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration
        )
    {
        string connStringTemplate = configuration.GetConnectionString("MongoDb")!;
        string connString = connStringTemplate
            .Replace("$MONGO_HOST", Environment.GetEnvironmentVariable("MONGO_HOST"))
            .Replace("$MONGO_PORT", Environment.GetEnvironmentVariable("MONGO_PORT"));

        services.AddSingleton<IMongoClient>(sp => new MongoClient(connString));
        services.AddSingleton(sp => sp.GetRequiredService<IMongoClient>().GetDatabase("OrdersDatabase"));

        services.AddScoped<IOrderRepository, OrderRepository>();

        return services;
    }
}
