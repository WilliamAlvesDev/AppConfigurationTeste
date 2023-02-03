using System;

namespace AppConfiguration.Manager
{
    public class AppConfigurationOptions
    {
        public string Namespace { get; set; } = string.Empty;

        public bool UseDistributedCache { get; set; } = true;

        public TimeSpan CacheExpiration { get; set; } = TimeSpan.FromSeconds(15);
        
        public string AzureAppConfigurationConnectionStringKey { get; set; } = "AzureAppConfiguration";
        
        public string RedisConnectionStringKey { get; set; } = "Redis";
    }
}
