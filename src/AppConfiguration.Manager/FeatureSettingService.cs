using Azure;
using Azure.Data.AppConfiguration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static System.String;

namespace AppConfiguration.Manager
{
    public class FeatureSettingService : IFeatureSettingService
    {
        private readonly ILogger<IFeatureSettingService> _logger;
        readonly AppConfigurationOptions _options;
        private readonly IAppConfigurationProvider _provider;
        private User _user;

        public FeatureSettingService(ILogger<IFeatureSettingService> logger, IAppConfigurationProvider provider, AppConfigurationOptions options)
        {
            _logger = logger;
            _options = options;
            _provider = provider ?? throw new ArgumentNullException(nameof(provider));
        }

        private async Task<ConfigurationSetting> GetOrCreateConfigurationSettingAsync(string id, string value, CancellationToken cancellationToken)
        {
            try
            {
                return await _provider.GetConfigurationSettingAsync(id, cancellationToken).ConfigureAwait(false);
            }
            catch (RequestFailedException ex)
            {
                if (ex.Status != 404) throw;

                return await _provider.SetConfigurationSettingAsync(id, value, cancellationToken)
                    .ConfigureAwait(false);
            }
        }

        private async Task<FeatureSetting> GetOrCreateFeatureSettingAsync(string id, CancellationToken cancellationToken)
        {
            var namespacePrefix = IsNullOrWhiteSpace(_options.Namespace) ? Empty : $"{_options.Namespace}:";

            var featureSetting = FeatureSetting.GetByAppConfigurationSettings(id.Replace(namespacePrefix, ""));

            var configurationSetting = await GetOrCreateConfigurationSettingAsync(id, featureSetting?.Value, cancellationToken)
                .ConfigureAwait(false);

            if (configurationSetting is null)
            {
                return featureSetting;
            }

            featureSetting ??= new FeatureSetting(configurationSetting.Key, Empty, configurationSetting.Value);

            featureSetting.AddPrefixToId(_options.Namespace)
                    .ChangeValue(configurationSetting.Value);

            return featureSetting;
        }

        public async Task<IEnumerable<FeatureSetting>> RetrieveAsync(CancellationToken cancellationToken)
        {
            var featureSettings = FeatureSetting.GetByAppConfigurationSettings();

            var featureSettingsTasks = featureSettings
                .Select(featureSetting => RetrieveAsync(featureSetting.Id, cancellationToken))
                .ToArray();

            await Task.WhenAll(featureSettingsTasks).ConfigureAwait(false);

            var pageableConfigurationSettings = _provider.GetConfigurationSetting($"{_options.Namespace}:*", cancellationToken);

            var result = featureSettingsTasks.Select(t => t.Result).ToList();

            foreach (var configurationSetting in pageableConfigurationSettings.ToArray())
            {
                if (result.Any(s => s.Id == configurationSetting.Key))
                {
                    continue;
                }

                result.Add(new FeatureSetting(configurationSetting.Key, Empty, configurationSetting.Value));
            }

            result.Sort((a, b) =>
            {
                if (a.Id == null && b.Id == null) return 0;

                return Compare(a.Id, b.Id, StringComparison.Ordinal);
            });

            return result;
        }

        public async Task<FeatureSetting> RetrieveAsync(string id, CancellationToken cancellationToken)
        {
            var namespacePrefix = IsNullOrWhiteSpace(_options.Namespace) ? Empty : $"{_options.Namespace}:";

            var featureSettingId = id.Contains(namespacePrefix) ? id : $"{namespacePrefix}{id}";

            return await GetOrCreateFeatureSettingAsync(featureSettingId, cancellationToken).ConfigureAwait(false);
        }

        public async Task<FeatureSetting> ChangeValueAsync(string id, string value, CancellationToken cancellationToken)
        {
            var featureSetting = await RetrieveAsync(id, cancellationToken).ConfigureAwait(false);

            var from = featureSetting?.Value;

            featureSetting.ChangeValue(value);

            var configurationSetting = await _provider.SetConfigurationSettingAsync(featureSetting.Id, featureSetting?.Value, cancellationToken).ConfigureAwait(false);

            _logger.LogInformation($"{_user.Name} ({_user.Email}) changed the {configurationSetting.Key} feature setting to value: from {from} to {value}");

            return featureSetting;
        }
        
        public async Task RemoveAsync(string id, CancellationToken cancellationToken)
        {
            await _provider.RemoveConfigurationSettingAsync(id, cancellationToken).ConfigureAwait(false);

            _logger.LogInformation($"{_user.Name} ({_user.Email}) removed the {id} feature setting");
        }

        public IFeatureSettingService SetUser(User user)
        {
            _user = user;

            return this;
        }
    }
}
