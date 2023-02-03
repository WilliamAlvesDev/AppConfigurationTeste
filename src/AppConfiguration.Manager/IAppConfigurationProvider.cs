using System;
using Azure.Data.AppConfiguration;
using System.Threading;
using System.Threading.Tasks;
using Azure;

namespace AppConfiguration.Manager
{
    public interface IAppConfigurationProvider
    {
        public TimeSpan CacheExpiration { get; }

        Pageable<ConfigurationSetting> GetConfigurationSetting(string keyFilter, CancellationToken cancellationToken);

        Task<ConfigurationSetting> GetConfigurationSettingAsync(string key, CancellationToken cancellationToken);

        Task<ConfigurationSetting> GetFeatureFlagConfigurationSettingAsync(string key,
            CancellationToken cancellationToken);

        Task<ConfigurationSetting> SetConfigurationSettingAsync(string key, string value,
            CancellationToken cancellationToken);

        Task<ConfigurationSetting> SetConfigurationSettingAsync(string key, string value, string contentType,
            CancellationToken cancellationToken);

        Task<ConfigurationSetting> SetFeatureFlagConfigurationSettingAsync(string id, string description, bool enabled,
            CancellationToken cancellationToken);

        Task RemoveConfigurationSettingAsync(string key, CancellationToken cancellationToken);
    }
}
