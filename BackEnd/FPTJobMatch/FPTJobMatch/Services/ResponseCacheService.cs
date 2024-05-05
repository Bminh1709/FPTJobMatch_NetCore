using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using StackExchange.Redis;

namespace FPTJobMatch.Services
{
    public class ResponseCacheService : IResponseCacheService
    {
        private readonly IDistributedCache _distributedCache; // Distributed cache for storing cached responses
        private readonly IConnectionMultiplexer _connectionMultiplexer; // Redis connection multiplexer for pattern-based cache removal

        public ResponseCacheService(IDistributedCache distributedCache, IConnectionMultiplexer connectionMultiplexer)
        {
            _distributedCache = distributedCache;
            _connectionMultiplexer = connectionMultiplexer;
        }

        // Method to asynchronously retrieve a cached response
        public async Task<string> GetCacheResponseAsync(string cacheKey)
        {
            // Retrieve cached response from distributed cache
            var cacheResponse = await _distributedCache.GetStringAsync(cacheKey);
            return string.IsNullOrEmpty(cacheResponse) ? null : cacheResponse; // Return cached response, or null if not found
        }

        // Method to asynchronously remove cached responses based on a pattern
        public async Task RemoveCacheResponseAsync(string pattern)
        {
            // Validate pattern parameter
            if (string.IsNullOrWhiteSpace(pattern))
                throw new ArgumentNullException(nameof(pattern), "Value cannot be null or whitespace");

            // Asynchronously iterate through keys matching the pattern and remove them from distributed cache
            await foreach (var key in GetKeyAsync(pattern + "*"))
            {
                await _distributedCache.RemoveAsync(key);
            }
        }

        // Asynchronous enumerable method to get keys matching a pattern
        private async IAsyncEnumerable<string> GetKeyAsync(string pattern)
        {
            // Validate pattern parameter
            if (string.IsNullOrWhiteSpace(pattern))
                throw new ArgumentNullException(nameof(pattern), "Value cannot be null or whitespace");

            // Iterate through endpoints and keys matching the pattern using Redis connection multiplexer
            foreach (var endPoint in _connectionMultiplexer.GetEndPoints())
            {
                var server = _connectionMultiplexer.GetServer(endPoint);
                foreach (var key in server.Keys(pattern: pattern))
                {
                    yield return key.ToString(); // Yield key as a string
                }
            }
        }

        // Method to asynchronously set a cached response
        public async Task SetCacheResponseAsync(string cacheKey, object response, TimeSpan timeOut)
        {
            // Skip caching if response is null
            if (response == null)
                return;

            // Serialize response object to JSON with CamelCase property names
            var serializerResponse = JsonConvert.SerializeObject(response, new JsonSerializerSettings()
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });

            // Set cached response in distributed cache with specified time-to-live
            await _distributedCache.SetStringAsync(cacheKey, serializerResponse, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = timeOut
            });
        }
    }
}