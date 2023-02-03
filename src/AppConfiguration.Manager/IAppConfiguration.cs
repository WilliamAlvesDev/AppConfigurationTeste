using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AppConfiguration.Manager
{
    public interface IAppConfiguration
    {
        Task<FeatureFlag> ToggleFeatureFlagAsync(FeatureFlag featureFlag, CancellationToken cancellationToken);

        Task<FeatureFlag> RetrieveFeatureFlagAsync(FeatureFlag featureFlag, CancellationToken cancellationToken);

        Task<FeatureFlag> RetrieveFeatureFlagAsync(string name, CancellationToken cancellationToken);

        Task<bool> IsEnabledAsync(FeatureFlag featureFlag, CancellationToken cancellationToken);

        bool IsEnabled(FeatureFlag featureFlag);

        Task<FeatureSetting> ChangeFeatureSettingAsync(FeatureSetting featureSetting, string value, CancellationToken cancellationToken);

        Task<FeatureSetting> RetrieveFeatureSettingAsync(FeatureSetting featureSetting, CancellationToken cancellationToken);

        Task<FeatureSetting> RetrieveFeatureSettingAsync(string name, CancellationToken cancellationToken);

        Task<IEnumerable<FeatureFlag>> RetrieveFeatureFlagsAsync(CancellationToken cancellationToken);

        Task<IEnumerable<FeatureSetting>> RetrieveFeatureSettingsAsync(CancellationToken cancellationToken);
    }
}