using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AppConfiguration.Manager
{
    public static class DistributedCacheExtensions
    {
        private static readonly JsonSerializerSettings CacheSerializerSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
        };

        public static Task SaveObjectAsJson<TObject>(this IDistributedCache distributedCache, TObject theObject, string key, TimeSpan cacheExpiration, CancellationToken cancellationToken)
        {
            var options = new DistributedCacheEntryOptions();

            if (cacheExpiration is { })
            {
                options.SetAbsoluteExpiration(cacheExpiration);
            }

            var serializedObject = JsonConvert.SerializeObject(theObject, CacheSerializerSettings);

            return distributedCache.SetStringAsync(key, serializedObject, options, cancellationToken);
        }

        public static async Task<TObject> DeserializeJsonObject<TObject>(this IDistributedCache distributedCache,
            string key, CancellationToken cancellationToken)
        {
            var cachedResultAsString =
                await distributedCache.GetStringAsync(key, cancellationToken).ConfigureAwait(false);

            if (string.IsNullOrWhiteSpace(cachedResultAsString))
            {
                return default;
            }

            var cachedData = JsonConvert.DeserializeObject<TObject>(cachedResultAsString);

            return cachedData;
        }
    }
}