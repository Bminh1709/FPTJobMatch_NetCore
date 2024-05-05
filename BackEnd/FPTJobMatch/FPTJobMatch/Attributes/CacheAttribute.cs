using FPTJobMatch.Configurations;
using FPTJobMatch.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Text;

namespace FPTJobMatch.Attributes
{
    public class CacheAttribute : Attribute, IAsyncActionFilter
    {
        private readonly int _timeToLiveSeconds; 

        public CacheAttribute(int timeToLiveSeconds = 1000)
        {
            _timeToLiveSeconds = timeToLiveSeconds; 
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // Retrieve Redis configuration from the request services
            var cacheConfiguration = context.HttpContext.RequestServices.GetRequiredService<RedisConfiguration>();

            // If caching is disabled, proceed with the action execution
            if (!cacheConfiguration.Enabled)
            {
                await next();
                return;
            }

            // Retrieve the response cache service from the request services
            var cacheService = context.HttpContext.RequestServices.GetRequiredService<IResponseCacheService>();

            // Generate a cache key based on the HTTP request
            var cacheKey = GenerateCacheKeyFromRequest(context.HttpContext.Request);

            // Attempt to retrieve the cached response using the generated cache key
            var cacheResponse = await cacheService.GetCacheResponseAsync(cacheKey);

            // If a cached response is found, set the content result with the cached response and return
            if (!string.IsNullOrEmpty(cacheResponse))
            {
                var contentResult = new ContentResult
                {
                    Content = cacheResponse,
                    ContentType = "application/json",
                    StatusCode = 200
                };
                context.Result = contentResult;
                return;
            }

            // Proceed with the action execution
            var executedContext = await next();

            // If the result is an OkObjectResult, cache the response using the IResponseCacheService
            if (executedContext.Result is OkObjectResult objectResult)
                await cacheService.SetCacheResponseAsync(cacheKey, objectResult.Value, TimeSpan.FromSeconds(_timeToLiveSeconds));
        }

        // Method to generate a cache key from the HTTP request
        private static string GenerateCacheKeyFromRequest(HttpRequest request)
        {
            var keyBuilder = new StringBuilder();
            keyBuilder.Append($"{request.Path}"); // Append the request path

            // Append query parameters sorted alphabetically to ensure consistent cache keys
            foreach (var (key, value) in request.Query.OrderBy(x => x.Key))
            {
                keyBuilder.Append($"|{key}-{value}");
            }

            return keyBuilder.ToString(); // Return the generated cache key
        }
    }
}
