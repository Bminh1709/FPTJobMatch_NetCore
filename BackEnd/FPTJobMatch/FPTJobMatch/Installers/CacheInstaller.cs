
using FPTJobMatch.Configurations;
using FPTJobMatch.Services;
using StackExchange.Redis;

namespace FPTJobMatch.Installers
{
    public class CacheInstaller : IInstaller
    {
        // Method to install cache-related services
        public void InstallServices(IServiceCollection services, IConfiguration configuration)
        {
            // Create an instance of RedisConfiguration to hold the settings
            var redisConfiguration = new RedisConfiguration();

            // Bind the RedisConfiguration section from appsettings.json to the instance
            configuration.GetSection("RedisConfiguration").Bind(redisConfiguration);

            // Add RedisConfiguration as a singleton service
            services.AddSingleton(redisConfiguration);

            // Check if Redis caching is enabled
            if (!redisConfiguration.Enabled)
            {
                // If caching is disabled, return without adding any further services
                return;
            }

            // Add Redis ConnectionMultiplexer as a singleton service
            services.AddSingleton<IConnectionMultiplexer>(_ => ConnectionMultiplexer.Connect(redisConfiguration.ConnectionString));

            // Add StackExchange.Redis cache service with the specified configuration
            services.AddStackExchangeRedisCache(options => options.Configuration = redisConfiguration.ConnectionString);

            // Add ResponseCacheService as a singleton service
            services.AddSingleton<IResponseCacheService, ResponseCacheService>();
        }
    }
}
