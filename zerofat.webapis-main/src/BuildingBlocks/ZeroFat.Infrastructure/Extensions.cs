using Hangfire.SqlServer;
using Microsoft.AspNetCore.Builder;
using ZeroFat.Infrastructure.Api.Auth;
using ZeroFat.Infrastructure.Api.Controllers;
using ZeroFat.Infrastructure.Api.Cors;
using ZeroFat.Infrastructure.Api.ExceptionHandlers;
using ZeroFat.Infrastructure.Api.Swagger;
using ZeroFat.Infrastructure.BackgroundProcessing;
using ZeroFat.Infrastructure.FeatureFlags;
using ZeroFat.Infrastructure.HealthChecks;
using ZeroFat.Infrastructure.Localization;
using ZeroFat.Infrastructure.Logging.Decorators;
using ZeroFat.Infrastructure.Observability;
using ZeroFat.Infrastructure.Paymob;
using ZeroFat.Infrastructure.Persistence;
using ZeroFat.Infrastructure.SMS;
using ZeroFat.Infrastructure.Storages;
using ZeroFat.Infrastructure.Stripe;
using ZeroFat.Infrastructure.Email;
using ZeroFat.Application.Common.Interfaces;


namespace ZeroFat.Infrastructure;
public static class Extensions
{
    public static void AddSharedInfrastructure(this WebApplicationBuilder builder)
    {
        builder.ConfigureSerilog();
        builder.ConfigureDatabase();

        builder.Services.AddInternalControllers();
        builder.Services.AddFeatureFlags();
        builder.Services.AddHttpClient();
        builder.Services.AddSwaggerModule();
        builder.Services.AddMvcCore();
        builder.Services.AddProblemDetails();
        builder.Services.AddPOLocalization(builder.Configuration);
        builder.Services.AddCorsPolicy(builder.Configuration);
        builder.Services.AddBackgroundProcessing(builder.Configuration);
        builder.Services.AddSharedHealthChecks(builder.Configuration);
        builder.Services.AddObservability();
        builder.Services.AddIPLogging();
        builder.Services.AddCurrentUser();
        builder.Services.AddPaymob(builder.Configuration);
        builder.Services.AddStripe(builder.Configuration);
        builder.Services.AddExceptionHandler<GlobalExceptionHandler>();


        var sms = builder.Configuration.GetSection("SMS").Get<SMSOptions>();
        builder.Services.AddSMSService(sms);

        var storage = builder.Configuration.GetSection("Storage").Get<StorageOptions>();
        builder.Services.AddStorageManager(storage);

        builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection(EmailSettings.SectionName));
        builder.Services.AddTransient<IEmailNotificationService, SmtpEmailNotificationService>();
        builder.Services.AddTransient<ISendSubscriptionPaymentLinkEmailJob, SmtpEmailNotificationService>();

    }

    public static void UseSharedInfrastructure(this WebApplication app)
    {
        app.UseRequestLocalization();
        app.UseHttpsRedirection();
        app.UseCorsPolicy();
        app.UseExceptionHandler();
        /// app.UseWebSockets(configuration);
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseSwaggerModule();
        // app.UseExceptionHandler();
        app.UseIPLogging();
        app.UseCurrentUser();
        app.UseFileStorage();
        app.UseSharedHealthChecks(app.Configuration);
        app.UseBackgroundProcessing(app.Configuration);
        app.UseRouting().UseEndpoints(endpoints =>
        {
            endpoints.MapDefaultControllerRoute();
            endpoints.MapControllers();
            endpoints.MapBackgroundProcessing();
            endpoints.MapSharedHealthChecks(app.Configuration);
        });
    }

    /// internal static IHostBuilder UseSharedInfrastructure(this IHostBuilder hostBuilder) = >hostBuilder.UseEvents();
}
