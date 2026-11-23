using System.Globalization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Scalar.AspNetCore;
using Serilog;
using Serilog.Exceptions;

namespace Shared.Framework.Hosting;

public static class SharedHosting
{
    public static ILogger CreateBootstrapLogger() =>
        new LoggerConfiguration()
            .MinimumLevel.Information()
            .WriteTo.Console(formatProvider: CultureInfo.InvariantCulture)
            .CreateLogger();

    public static void EnableSerilogSelfLog() =>
        Serilog.Debugging.SelfLog.Enable(message => Console.WriteLine($"[SERILOG ERROR] {message}"));

    public static IServiceCollection AddSharedSerilog(
        this IServiceCollection services,
        IConfiguration configuration,
        string serviceName,
        Action<LoggerConfiguration>? configure = null)
    {
        services.AddSerilog((serviceProvider, loggerConfiguration) =>
        {
            loggerConfiguration
                .ReadFrom.Configuration(configuration)
                .ReadFrom.Services(serviceProvider)
                .Enrich.FromLogContext()
                .Enrich.WithExceptionDetails()
                .Enrich.WithProperty("ServiceName", serviceName);

            configure?.Invoke(loggerConfiguration);
        });

        return services;
    }

    public static IServiceCollection AddSharedWebDefaults(this IServiceCollection services)
    {
        services.Configure<ApiBehaviorOptions>(options =>
            options.SuppressModelStateInvalidFilter = true);

        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddOpenApi();
        services.AddHealthChecks();

        return services;
    }

    public static WebApplication UseSharedSerilogRequestLogging(this WebApplication app)
    {
        app.UseSerilogRequestLogging();
        return app;
    }

    public static WebApplication MapSharedApiDocs(this WebApplication app)
    {
        if (!app.Environment.IsProduction())
        {
            app.MapOpenApi();
            app.MapScalarApiReference();
        }

        return app;
    }

    public static WebApplication MapSharedHealthChecks(this WebApplication app, string pattern = "/health")
    {
        app.MapHealthChecks(pattern);
        return app;
    }
}
