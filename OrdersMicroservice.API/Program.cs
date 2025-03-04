using FluentValidation.AspNetCore;
using OrdersMicroservice.API.Middlewares;
using OrdersMicroservice.Core;
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

builder.Services.AddControllers();

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

// http pipleline

var app = builder.Build();

app.UseExceptionHandlingMiddleware();

app.UseHttpsRedirection();

app.UseRouting();

app.UseCors();

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
