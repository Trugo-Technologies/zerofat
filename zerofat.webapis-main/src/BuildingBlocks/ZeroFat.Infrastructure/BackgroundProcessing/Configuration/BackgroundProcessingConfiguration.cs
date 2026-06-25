namespace ZeroFat.Infrastructure.BackgroundProcessing.Configuration;

using Ardalis.GuardClauses;

internal sealed class BackgroundProcessingConfiguration
{
    private const string BackgroundProcessing = "HangfireSettings:Storage";

    private readonly IConfigurationSection _configurationSection;
    public BackgroundProcessingConfiguration(IConfiguration configuration)
    {
        _configurationSection = configuration.GetSection(BackgroundProcessing);
    }

    public string StorageProvider => Guard.Against.NullOrEmpty(_configurationSection.GetValue<string>(nameof(StorageProvider)), nameof(StorageProvider));
    public string ConnectionString => Guard.Against.NullOrEmpty(_configurationSection.GetValue<string>(nameof(ConnectionString)), nameof(ConnectionString));

}
