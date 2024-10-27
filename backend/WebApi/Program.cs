using DotNetEnv;
using FastEndpoints;
using Serilog;
using WebApi;
using WebApi.Common.Infrastructure;
using WebApi.Common.Options;

var builder = WebApplication.CreateBuilder(args);

const string appSettingsPath = "Properties/appsettings.json";

// other environments should not contain .env files
if (builder.Environment.IsDevelopment())
{
    Env.TraversePath().Load();
}

builder.Configuration
    .AddEnvironmentVariables()
    .AddJsonFile(appSettingsPath)
    .Build();

builder.Services
    .ConfigureAndValidate<PostgresOptions>()
    .AddScoped<IStartupService, MigrateDb>()
    .AddDbContext<AppDbContext>()
    .AddFastEndpoints()
    .AddOpenApi();

builder.Host
    .UseSerilog(
    (_, _, loggerConfiguration) =>
    {
        loggerConfiguration
            .ReadFrom.Configuration(
                new ConfigurationBuilder()
                    .AddJsonFile(appSettingsPath)
                    .Build())
            .Enrich.FromLogContext()
            .WriteTo.Console();
    }
);

var app = builder.Build();

using (var startupScope = app.Services.CreateScope())
{
    var startupServices = startupScope.ServiceProvider.GetServices<IStartupService>();
    await Task.WhenAll(startupServices.Select(service => service.Run()));
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app
    .UseHttpsRedirection()
    .UseFastEndpoints();
    
await app.RunAsync();