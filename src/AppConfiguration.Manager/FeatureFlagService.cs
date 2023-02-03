using Azure;
using Azure.Data.AppConfiguration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static System.String;

namespace AppConfiguration.Manager
{
    public class FeatureFlagService : IFeatureFlagService
    {
        private readonly ILogger<IFeatureFlagService> _logger;
        private readonly AppConfigurationOptions _options;
        private readonly IAppConfigurationProvider _provider;

        private User _user;

        public FeatureFlagService(ILogger<FeatureFlagService> logger, IAppConfigurationProvider provider, AppConfigurationOptions options)
        {
            _logger = logger;
            _options = options;
            _provider = provider ?? throw new ArgumentNullException(nameof(provider));
        }

        public FeatureFlagService(IAppConfigurationProvider provider, AppConfigurationOptions options)
        {
            _options = options;
            _provider = provider ?? throw new ArgumentNullException(nameof(provider));
        }

        private async Task<ConfigurationSetting> GetOrCreateFeatureFlagConfigurationSettingAsync(string id, string description, bool enabled, CancellationToken cancellationToken)
        {
            try
            {
                return await _provider.GetFeatureFlagConfigurationSettingAsync(id, cancellationToken).ConfigureAwait(false);
            }
            catch (RequestFailedException ex)
            {
                if (ex.Status != 404) throw;

                return await _provider.SetFeatureFlagConfigurationSettingAsync(id, description, enabled, cancellationToken)
                    .ConfigureAwait(false);
            }
        }

        private async Task<FeatureFlag> GetOrCreateFeatureFlagAsync(string id, CancellationToken cancellationToken)
        {
            var namespacePrefix = IsNullOrWhiteSpace(_options.Namespace) ? Empty : $"{_options.Namespace}:";

            var featureFlag = FeatureFlag.GetByAppConfigurationSettings(id.Replace(namespacePrefix, ""));

            if (featureFlag is null)
            {
                return null;
            }

            var configurationSetting = await GetOrCreateFeatureFlagConfigurationSettingAsync(id, featureFlag.Description, featureFlag.Enabled, cancellationToken)
                .ConfigureAwait(false);

            if (configurationSetting is null)
            {
                return featureFlag;
            }

            var featureFlagValue = JsonConvert.DeserializeObject<FeatureFlagValue>(configurationSetting.Value);

            if (featureFlagValue is { })
            {
                featureFlag.AddPrefixToId(_options.Namespace)
                    .ChangeEnabled(featureFlagValue.Enabled);
            }

            return featureFlag;
        }

        public async Task<IEnumerable<FeatureFlag>> RetrieveAsync(CancellationToken cancellationToken)
        {
            var featureFlags = FeatureFlag.GetByAppConfigurationSettings();

            var featureFlagsTasks = featureFlags
                .Select(featureFlag => RetrieveAsync(featureFlag.Id, cancellationToken))
                .ToArray();

            await Task.WhenAll(featureFlagsTasks).ConfigureAwait(false);

            var result = featureFlagsTasks.Select(t => t.Result).ToList();

            var pageableConfigurationSettings = _provider.GetConfigurationSetting($"{Constants.Prefix}/{_options.Namespace}*", cancellationToken);

            foreach (var configurationSetting in pageableConfigurationSettings.ToArray())
            {
                if (result.Any(s => configurationSetting.Key.Contains(s.Id)))
                {
                    continue;
                }

                var featureFlagValue = JsonConvert.DeserializeObject<FeatureFlagValue>(configurationSetting.Value);

                if (featureFlagValue is null)
                {
                    continue;
                }

                result.Add(new FeatureFlag(configurationSetting.Key.Replace(Constants.Prefix, Empty), featureFlagValue.Description, featureFlagValue.Enabled));
            }

            result.Sort((a, b) =>
            {
                if (a.Id == null && b.Id == null) return 0;

                return Compare(a.Id, b.Id, StringComparison.Ordinal);
            });

            return result;
        }

        public async Task<FeatureFlag> RetrieveAsync(string id, CancellationToken cancellationToken)
        {
            var namespacePrefix = IsNullOrWhiteSpace(_options.Namespace) ? Empty : $"{_options.Namespace}:";

            var featureFlagId = id.Contains(namespacePrefix) ? id : $"{namespacePrefix}{id}";

            return await GetOrCreateFeatureFlagAsync(featureFlagId, cancellationToken).ConfigureAwait(false);
        }

        public async Task<FeatureFlag> ToggleAsync(string id, CancellationToken cancellationToken)
        {
            var namespacePrefix = IsNullOrWhiteSpace(_options.Namespace) ? Empty : $"{_options.Namespace}:";

            var featureFlagId = id.Contains(namespacePrefix) ? id : $"{namespacePrefix}{id}";

            var featureFlag = await RetrieveAsync(featureFlagId, cancellationToken).ConfigureAwait(false);

            featureFlag.Toggle();

            _ = await _provider.SetFeatureFlagConfigurationSettingAsync(featureFlag.Id, featureFlag.Description, featureFlag.Enabled, cancellationToken)
                .ConfigureAwait(false);

            _logger.LogInformation("{UserName} ({UserEmail}) toggled the {Id} feature flag to value: {Enabled}", _user.Name, _user.Email, featureFlag.Id, featureFlag.Enabled);

            return featureFlag;
        }

        public async Task<FeatureFlag> ChangeEnabledAsync(string id, bool enabled, CancellationToken cancellationToken)
        {
            var namespacePrefix = IsNullOrWhiteSpace(_options.Namespace) ? Empty : $"{_options.Namespace}:";

            var featureFlagId = id.Contains(namespacePrefix) ? id : $"{namespacePrefix}{id}";

            var featureFlag = await RetrieveAsync(featureFlagId, cancellationToken).ConfigureAwait(false);

            featureFlag.ChangeEnabled(enabled);

            _ = await _provider.SetFeatureFlagConfigurationSettingAsync(featureFlag.Id, featureFlag.Description, featureFlag.Enabled, cancellationToken)
                .ConfigureAwait(false);

            _logger.LogInformation("{UserName} ({UserEmail}) change the {Id} feature flag to value: {Enabled}", _user.Name, _user.Email, featureFlag.Id, featureFlag.Enabled);

            return featureFlag;
        }

        public IFeatureFlagService SetUser(User user)
        {
            _user = user;

            return this;
        }

        public async Task RemoveAsync(string id, CancellationToken cancellationToken)
        {
            var namespacePrefix = IsNullOrWhiteSpace(_options.Namespace) ? Empty : $"{_options.Namespace}:";

            var featureFlagId = id.Contains(namespacePrefix) ? id : $"{namespacePrefix}{id}";

            await _provider.RemoveConfigurationSettingAsync(featureFlagId, cancellationToken).ConfigureAwait(false);

            _logger.LogInformation($"{_user.Name} ({_user.Email}) removed the {id} feature flag");
        }

        public async Task<bool> IsEnabledAsync(string id, CancellationToken cancellationToken)
        {
            var featureFlag = await RetrieveAsync(id, cancellationToken).ConfigureAwait(false);

            return featureFlag?.Enabled ?? false;
        }
    }
}
