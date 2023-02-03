using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AppConfiguration.Manager
{
    public class AppConfiguration : IAppConfiguration
    {
        private readonly IFeatureFlagService _featureFlagService;
        private readonly IFeatureSettingService _featureSettingService;

        public AppConfiguration(IFeatureFlagService featureFlagService, IFeatureSettingService featureSettingService)
        {
            _featureFlagService = featureFlagService;
            _featureSettingService = featureSettingService;
        }

        public async Task<FeatureFlag> ToggleFeatureFlagAsync(FeatureFlag featureFlag, CancellationToken cancellationToken) =>
            await _featureFlagService.ToggleAsync(featureFlag.Id, cancellationToken).ConfigureAwait(false);
        
        public async Task<IEnumerable<FeatureFlag>> RetrieveFeatureFlagsAsync(CancellationToken cancellationToken) =>
            await _featureFlagService.RetrieveAsync(cancellationToken).ConfigureAwait(false);
        
        public async Task<FeatureFlag> RetrieveFeatureFlagAsync(string name, CancellationToken cancellationToken) =>
            await _featureFlagService.RetrieveAsync(name, cancellationToken).ConfigureAwait(false);

        public async Task<FeatureFlag> RetrieveFeatureFlagAsync(FeatureFlag featureFlag, CancellationToken cancellationToken) =>
            await RetrieveFeatureFlagAsync(featureFlag.Id, cancellationToken).ConfigureAwait(false);

        public async Task<bool> IsEnabledAsync(FeatureFlag featureFlag, CancellationToken cancellationToken) =>
            await _featureFlagService.IsEnabledAsync(featureFlag.Id, cancellationToken).ConfigureAwait(false);

        public bool IsEnabled(FeatureFlag featureFlag) =>
            _featureFlagService.IsEnabledAsync(featureFlag.Id, default).GetAwaiter().GetResult();

        public async Task<FeatureSetting> ChangeFeatureSettingAsync(FeatureSetting featureSetting, string value, CancellationToken cancellationToken) =>
            await _featureSettingService.ChangeValueAsync(featureSetting.Id, value, cancellationToken).ConfigureAwait(false);

        public async Task<FeatureSetting> RetrieveFeatureSettingAsync(FeatureSetting featureSetting, CancellationToken cancellationToken) =>
            await RetrieveFeatureSettingAsync(featureSetting.Id, cancellationToken).ConfigureAwait(false);

        public async Task<FeatureSetting> RetrieveFeatureSettingAsync(string name, CancellationToken cancellationToken) =>
            await _featureSettingService.RetrieveAsync(name, cancellationToken).ConfigureAwait(false);
        
        public async Task<IEnumerable<FeatureSetting>> RetrieveFeatureSettingsAsync(CancellationToken cancellationToken) =>
            await _featureSettingService.RetrieveAsync(cancellationToken).ConfigureAwait(false);
    }
}