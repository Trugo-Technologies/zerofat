namespace ZeroFat.Infrastructure.HealthChecks.Configuration;

using Ardalis.GuardClauses;

internal sealed class HealthCheckConfiguration(IConfiguration configuration)
{
    private const string HealthCheckConfig = "HealthChecksUI";
    private readonly IConfigurationSection _configurationSection = configuration.GetSection(HealthCheckConfig);

    public string UIPath => Guard.Against.NullOrEmpty(_configurationSection.GetValue<string>(nameof(UIPath)), nameof(UIPath));
    public string ApiPath => Guard.Against.NullOrEmpty(_configurationSection.GetValue<string>(nameof(ApiPath)), nameof(ApiPath));
    public string RootPath => Guard.Against.NullOrEmpty(_configurationSection.GetValue<string>(nameof(RootPath)), nameof(RootPath));

}
