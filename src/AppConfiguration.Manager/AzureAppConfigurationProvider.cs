using Azure;
using Azure.Data.AppConfiguration;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AppConfiguration.Manager
{
    public sealed class AzureAppConfigurationProvider : IAppConfigurationProvider
    {
        private readonly ConfigurationClient _client;
        private readonly JsonSerializerSettings _jsonSerializerSettings;

        private const string ContentType = "application/vnd.microsoft.appconfig.ff+json;charset=utf-8";

        public AzureAppConfigurationProvider(string connectionString, AppConfigurationOptions options)
        {
            if (options is null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            _client = new ConfigurationClient(connectionString);

            _jsonSerializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new CamelCaseNamingStrategy()
                },
                Formatting = Formatting.Indented
            };

            CacheExpiration = options.CacheExpiration;
        }

        public TimeSpan CacheExpiration { get; }

        public Pageable<ConfigurationSetting> GetConfigurationSetting(string keyFilter, CancellationToken cancellationToken) =>
            _client.GetConfigurationSettings(new SettingSelector { KeyFilter = keyFilter }, cancellationToken);

        public async Task<ConfigurationSetting> GetConfigurationSettingAsync(string key, CancellationToken cancellationToken) =>
            await _client.GetConfigurationSettingAsync(key, cancellationToken: cancellationToken).ConfigureAwait(false);

        public async Task<ConfigurationSetting> GetFeatureFlagConfigurationSettingAsync(string key, CancellationToken cancellationToken) =>
            await GetConfigurationSettingAsync($"{Constants.Prefix}/{key}", cancellationToken).ConfigureAwait(false);

        public async Task<ConfigurationSetting> SetConfigurationSettingAsync(string key, string value, CancellationToken cancellationToken) =>
            await _client.SetConfigurationSettingAsync(
                new ConfigurationSetting(key, value),
                cancellationToken: cancellationToken
            ).ConfigureAwait(false);

        public async Task<ConfigurationSetting> SetConfigurationSettingAsync(string key, string value, string contentType, CancellationToken cancellationToken) =>
            await _client.SetConfigurationSettingAsync(
                new ConfigurationSetting(key, value) { ContentType = contentType },
                cancellationToken: cancellationToken
            ).ConfigureAwait(false);

        public async Task<ConfigurationSetting> SetFeatureFlagConfigurationSettingAsync(string id, string description, bool enabled, CancellationToken cancellationToken)
        {
            var key = $"{Constants.Prefix}/{id}";

            var featureFlagValue = new FeatureFlagValue(id, description, enabled);

            var value = JsonConvert.SerializeObject(featureFlagValue, _jsonSerializerSettings);

            var configurationSetting = await SetConfigurationSettingAsync(key, value, ContentType, cancellationToken)
                .ConfigureAwait(false);

            return configurationSetting;
        }

        public async Task RemoveConfigurationSettingAsync(string id, CancellationToken cancellationToken)
        {
            var key = $"{Constants.Prefix}/{id}";

            var configurationSetting = await _client.GetConfigurationSettingAsync(key, cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            _ = await _client.DeleteConfigurationSettingAsync(
                configurationSetting,
                cancellationToken: cancellationToken
            ).ConfigureAwait(false);
        }
    }
}
