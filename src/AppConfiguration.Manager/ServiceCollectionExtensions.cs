using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using Microsoft.Extensions.Logging;

namespace AppConfiguration.Manager
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAppConfiguration(this IServiceCollection services, IConfiguration configuration, Action<AppConfigurationOptions> setup)
        {
            var options = new AppConfigurationOptions();
            setup?.Invoke(options);

            if (options.UseDistributedCache && options.CacheExpiration.Ticks > 0)
            {
                var redisConnectionString = configuration.GetConnectionString(options.RedisConnectionStringKey);
                services.AddStackExchangeRedisCache(opt =>
                {
                    opt.Configuration = redisConnectionString;
                });
            }

            var azureAppConfigurationConnection = configuration.GetConnectionString(options.AzureAppConfigurationConnectionStringKey);

            services.AddSingleton<IAppConfigurationProvider>(new AzureAppConfigurationProvider(azureAppConfigurationConnection, options));

            if (options.UseDistributedCache && options.CacheExpiration.Ticks > 0)
            {
                services.Decorate<IAppConfigurationProvider, RedisAppConfigurationProvider>();
            }

            services.AddSingleton<IAppConfiguration, AppConfiguration>();
            services.AddSingleton<IFeatureFlagService, FeatureFlagService>(sp =>
            {
                var provider = sp.GetRequiredService<IAppConfigurationProvider>();
                var logger = sp.GetRequiredService<ILogger<FeatureFlagService>>();

                return new FeatureFlagService(logger, provider, options);

            });
            services.AddSingleton<IFeatureSettingService>(sp =>
            {
                var provider = sp.GetRequiredService<IAppConfigurationProvider>();
                var logger = sp.GetRequiredService<ILogger<IFeatureSettingService>>();

                return new FeatureSettingService(logger, provider, options);
            });

            services.AddAzureAppConfiguration();

            return services;
        }
    }
}
