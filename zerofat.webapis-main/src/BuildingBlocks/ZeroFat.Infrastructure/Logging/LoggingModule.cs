using global::Serilog;
using ZeroFat.Infrastructure.Behaviours;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.EventLog;
using OpenTelemetry.Logs;
using OpenTelemetry.Resources;
using Serilog.Events;
using Serilog.Filters;

namespace ZeroFat.Infrastructure.Logging.Decorators;
internal static class LoggingModule
{
    private const string ContentType = "application/javascript";

    internal static WebApplicationBuilder ConfigureSerilog(this WebApplicationBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        builder.Host.UseSerilog((context, logger) =>
        {
            logger.ReadFrom.Configuration(context.Configuration);
            logger.Enrich.FromLogContext();
            logger
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
                .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Error)
                .MinimumLevel.Override("Hangfire", LogEventLevel.Warning)
                .Filter.ByExcluding(Matching.FromSource("Microsoft.AspNetCore.Diagnostics.ExceptionHandlerMiddleware"));
        });
        return builder;
    }

    public static WebApplicationBuilder UseInnovateProLogger(this WebApplicationBuilder builder)
    {
        builder.Services.AddOptions<LoggingOptions>().BindConfiguration(LoggingOptions.SectionName);
        var options = builder.Configuration.GetSection(LoggingOptions.SectionName).Get<LoggingOptions>();

        builder.Logging.ClearProviders();
        builder.Logging.AddSerilog();
        if (options?.EventLog != null && options.EventLog.IsEnabled && OperatingSystem.IsWindows())
        {
            builder.Logging.AddEventLog(new EventLogSettings
            {
                LogName = options.EventLog.LogName,
                SourceName = options.EventLog.SourceName,

            });
        }

        if (options?.OpenTelemetry?.IsEnabled ?? false)
        {
            var resourceBuilder = ResourceBuilder.CreateDefault().AddService(options.OpenTelemetry.ServiceName);

            builder.Logging.AddOpenTelemetry(configure =>
            {
                configure.SetResourceBuilder(resourceBuilder);

                configure.IncludeScopes = true;
                configure.IncludeFormattedMessage = true;

                configure.AddOtlpExporter(otlpOptions =>
                {
                    otlpOptions.Endpoint = new Uri(options.OpenTelemetry.Otlp.Endpoint);
                });
            });
        }

        // For AppAzure
        /// builder.Logging.AddAzureWebAppDiagnostics();
        Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(builder.Configuration).CreateLogger();

        return builder;
    }

    internal static void AddIPLogging(this IServiceCollection services)
    {
        services.AddHttpLogging(logging =>
        {
            logging.LoggingFields = HttpLoggingFields.RequestPropertiesAndHeaders |
                                    HttpLoggingFields.RequestBody;
            logging.MediaTypeOptions.AddText(ContentType);
        });
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
    }

    internal static void UseIPLogging(this IApplicationBuilder app) => app.UseHttpLogging();
}
