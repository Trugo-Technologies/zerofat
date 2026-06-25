namespace ZeroFat.Infrastructure.FeatureFlags;

public interface IFeatureFlagsChecker
{
    Task<bool> IsEnabledAsync(FeatureFlag featureFlag, CancellationToken cancellationToken = default);
    bool IsEnabled(FeatureFlag featureFlag);
}
