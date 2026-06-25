using Hangfire;
using Serilog;
using ZeroFat.ClientPortal.Application.Contracts;
using ZeroFat.Infrastructure.BackgroundProcessing.Contracts;
using ZeroFat.Infrastructure.Logging.Serilog;
using ZeroFat.NutriPlan.Application.Contracts;
using ZeroFat.WebAPIs;
using ZeroFat.WebAPIs.Configurations;

StaticLogger.EnsureInitialized();
Log.Information("server booting up..");
try
{
    WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

    builder.AddConfigurations();
    builder.AddModules(builder.Configuration);

    WebApplication app = builder.Build();
    app.UseModules();

    app.Run();


}
catch (Exception ex) when (!ex.GetType().Name.Equals("HostAbortedException", StringComparison.Ordinal))
{
    StaticLogger.EnsureInitialized();
    Log.Fatal(ex, "unhandled exception");
}
finally
{
    StaticLogger.EnsureInitialized();
    Log.Information("server shutting down..");
    Log.CloseAndFlush();
}


