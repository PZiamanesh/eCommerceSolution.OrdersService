using FluentValidation.AspNetCore;
using OrdersMicroservice.API.Middlewares;
using OrdersMicroservice.Core;
using OrdersMicroservice.Core.HttpClients;
using OrdersMicroservice.Infrastructure;
using System.Reflection;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;

Assembly[] assemblies = [
    typeof(CoreConfigs).Assembly,
    typeof(InfraConfigs).Assembly,
];

// ioc container

builder.Services.AddInfrastructureServices(configuration);
builder.Services.AddCoreServices(configuration);

builder.Services.AddControllers()
    .AddJsonOptions(cnfg =>
    {
        cnfg.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
        cnfg.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddFluentValidationAutoValidation();

builder.Services.AddAutoMapper(assemblies);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder => {
        builder.WithOrigins("http://localhost:4200")
        .AllowAnyMethod()
        .AllowAnyHeader();
    });
});

builder.Services.AddHttpClient<UsersMicroserviceClient>(clientConfig =>
{
    clientConfig.BaseAddress = new Uri($"http://{builder.Configuration["UsersMicroserviceHost"]}:{builder.Configuration["UsersMicroservicePort"]}");
});

builder.Services.AddHttpClient<ProductsMicroserviceClient>(clientConfig =>
{
    clientConfig.BaseAddress = new Uri($"http://{builder.Configuration["ProductsMicroserviceHost"]}:{builder.Configuration["ProductsMicroservicePort"]}");
});

// http pipleline

var app = builder.Build();

app.UseExceptionHandlingMiddleware();

//app.UseHttpsRedirection();

app.UseRouting();

app.UseCors();

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
