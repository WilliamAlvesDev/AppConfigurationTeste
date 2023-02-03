using System;
using System.Threading;
using System.Threading.Tasks;
using Azure;
using Azure.Data.AppConfiguration;
using Microsoft.Extensions.Caching.Distributed;

namespace AppConfiguration.Manager
{
    public sealed class RedisAppConfigurationProvider : IAppConfigurationProvider
    {
        private readonly IDistributedCache _cache;
        private readonly IAppConfigurationProvider _provider;
        
        public RedisAppConfigurationProvider(IAppConfigurationProvider inner, IDistributedCache cache)
        {
            _cache = cache;
            _provider = inner;
        }

        public TimeSpan CacheExpiration => _provider.CacheExpiration;

        public Pageable<ConfigurationSetting> GetConfigurationSetting(string keyFilter,
            CancellationToken cancellationToken) =>
            _provider.GetConfigurationSetting(keyFilter, cancellationToken);

        public async Task<ConfigurationSetting> GetConfigurationSettingAsync(string key, CancellationToken cancellationToken)
        {
            var cached = await _cache.DeserializeJsonObject<ConfigurationSetting>(key, cancellationToken)
                .ConfigureAwait(false);

            if (cached is { })
            {
                return cached;
            }
            
            var configurationSetting = await _provider.GetConfigurationSettingAsync(key, cancellationToken)
                .ConfigureAwait(false);

            if (configurationSetting is { })
            {
                await _cache.SaveObjectAsJson(configurationSetting, $"{configurationSetting.Key}", CacheExpiration, cancellationToken);
            }

            return configurationSetting;
        }

        public async Task<ConfigurationSetting> GetFeatureFlagConfigurationSettingAsync(string key, CancellationToken cancellationToken)
        {
            var cached = await _cache.DeserializeJsonObject<ConfigurationSetting>($"{Constants.Prefix}/{key}", cancellationToken)
                .ConfigureAwait(false);

            if (cached is { })
            {
                return cached;
            }

            var configurationSetting = await _provider.GetFeatureFlagConfigurationSettingAsync(key, cancellationToken)
                .ConfigureAwait(false);

            if (configurationSetting is { })
            {
                await _cache.SaveObjectAsJson(configurationSetting, $"{configurationSetting.Key}", CacheExpiration, cancellationToken);
            }

            return configurationSetting;
        }

        public async Task<ConfigurationSetting> SetConfigurationSettingAsync(
            string key, 
            string value, 
            string contentType, 
            CancellationToken cancellationToken)
        {
            var configurationSetting = await _provider.SetConfigurationSettingAsync(key, value, contentType, cancellationToken)
                .ConfigureAwait(false);

            await _cache.SaveObjectAsJson(configurationSetting, $"{configurationSetting.Key}", CacheExpiration, cancellationToken);

            return configurationSetting;
        }

        public async Task<ConfigurationSetting> SetFeatureFlagConfigurationSettingAsync(
            string id, 
            string description, 
            bool enabled, 
            CancellationToken cancellationToken)
        {
            var configurationSetting = await _provider.SetFeatureFlagConfigurationSettingAsync(id, description, enabled, cancellationToken);
            
            await _cache.SaveObjectAsJson(configurationSetting, $"{configurationSetting.Key}", CacheExpiration, cancellationToken);

            return configurationSetting;
        }

        public async Task RemoveConfigurationSettingAsync(string key, CancellationToken cancellationToken)
        {
            await _provider.RemoveConfigurationSettingAsync(key, cancellationToken);

            await _cache.RemoveAsync(key, cancellationToken);
        }

        public async Task<ConfigurationSetting> SetConfigurationSettingAsync(string key, string value, CancellationToken cancellationToken)
        {
            var configurationSetting = await _provider.SetConfigurationSettingAsync(key, value, cancellationToken)
                .ConfigureAwait(false);

            await _cache.SaveObjectAsJson(configurationSetting, $"{configurationSetting.Key}", CacheExpiration, cancellationToken);

            return configurationSetting;
        }
    }
}
