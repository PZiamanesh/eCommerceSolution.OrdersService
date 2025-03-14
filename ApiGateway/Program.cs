using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Polly;

var builder = WebApplication.CreateBuilder(args);

// ioc container

builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);

builder.Services
    .AddOcelot()
    .AddPolly();


// request pipeline

var app = builder.Build();

await app.UseOcelot();

app.Run();
