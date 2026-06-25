using Ardalis.GuardClauses;

namespace ZeroFat.Infrastructure.Api.Cors.Configurations;

internal sealed class CorsConfiguration
{
    private const string Cors = "CorsSettings";

    private readonly IConfigurationSection _configuration;
    internal CorsConfiguration(IConfiguration configuration)
    {
        _configuration = configuration.GetSection(Cors);
    }
    public string Production => Guard.Against.NullOrEmpty(_configuration.GetValue<string>(nameof(Production)), nameof(Production));
    public string Development => Guard.Against.NullOrEmpty(_configuration.GetValue<string>(nameof(Development)), nameof(Development));
}
