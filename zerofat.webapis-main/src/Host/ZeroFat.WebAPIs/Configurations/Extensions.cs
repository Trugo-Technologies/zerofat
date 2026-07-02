namespace ZeroFat.WebAPIs.Configurations;

internal static class Startup
{
    internal static WebApplicationBuilder AddConfigurations(this WebApplicationBuilder host)
    {
        const string configurationsDirectory = "Configurations";
        var environmentName = host.Environment.EnvironmentName;

        host.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
        host.Configuration.AddJsonFile($"appsettings.{environmentName}.json", optional: true, reloadOnChange: true);

        host.Configuration.AddJsonFile($"{configurationsDirectory}/hangfire.json", optional: false, reloadOnChange: true);
        host.Configuration.AddJsonFile($"{configurationsDirectory}/hangfire.{environmentName}.json", optional: true, reloadOnChange: true);

        host.Configuration.AddJsonFile($"{configurationsDirectory}/healthcheck.json", optional: false, reloadOnChange: true);
        host.Configuration.AddJsonFile($"{configurationsDirectory}/healthcheck.{environmentName}.json", optional: true, reloadOnChange: true);

        host.Configuration.AddJsonFile($"{configurationsDirectory}/cors.json", optional: false, reloadOnChange: true);
        host.Configuration.AddJsonFile($"{configurationsDirectory}/cors.{environmentName}.json", optional: true, reloadOnChange: true);

        host.Configuration.AddJsonFile($"{configurationsDirectory}/logger.json", optional: false, reloadOnChange: true);
        host.Configuration.AddJsonFile($"{configurationsDirectory}/logger.{environmentName}.json", optional: true, reloadOnChange: true);

        host.Configuration.AddJsonFile($"{configurationsDirectory}/security.json", optional: false, reloadOnChange: true);
        host.Configuration.AddJsonFile($"{configurationsDirectory}/security.{environmentName}.json", optional: true, reloadOnChange: true);

        host.Configuration.AddJsonFile($"{configurationsDirectory}/modules.json", optional: false, reloadOnChange: true);
        host.Configuration.AddJsonFile($"{configurationsDirectory}/modules.{environmentName}.json", optional: true, reloadOnChange: true);

        host.Configuration.AddJsonFile($"{configurationsDirectory}/localization.json", optional: false, reloadOnChange: true);
        host.Configuration.AddJsonFile($"{configurationsDirectory}/localization.{environmentName}.json", optional: true, reloadOnChange: true);

        host.Configuration.AddJsonFile($"{configurationsDirectory}/paymob.json", optional: false, reloadOnChange: true);
        host.Configuration.AddJsonFile($"{configurationsDirectory}/paymob.{environmentName}.json", optional: true, reloadOnChange: true);

        host.Configuration.AddJsonFile($"{configurationsDirectory}/stripe.json", optional: false, reloadOnChange: true);
        host.Configuration.AddJsonFile($"{configurationsDirectory}/stripe.{environmentName}.json", optional: true, reloadOnChange: true);

        host.Configuration.AddJsonFile($"{configurationsDirectory}/storage.json", optional: false, reloadOnChange: true);
        host.Configuration.AddJsonFile($"{configurationsDirectory}/storage.{environmentName}.json", optional: true, reloadOnChange: true);

        host.Configuration.AddJsonFile($"{configurationsDirectory}/sms.json", optional: false, reloadOnChange: true);
        host.Configuration.AddJsonFile($"{configurationsDirectory}/sms.{environmentName}.json", optional: true, reloadOnChange: true);

        host.Configuration.AddJsonFile($"{configurationsDirectory}/email.json", optional: true, reloadOnChange: true);
        host.Configuration.AddJsonFile($"{configurationsDirectory}/email.{environmentName}.json", optional: true, reloadOnChange: true);

        host.Configuration.AddJsonFile($"{configurationsDirectory}/pushnotification.json", optional: true, reloadOnChange: true);
        host.Configuration.AddJsonFile($"{configurationsDirectory}/pushnotification.{environmentName}.json", optional: true, reloadOnChange: true);

        host.Configuration.AddEnvironmentVariables();


        return host;
    }
}
