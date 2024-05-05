namespace FPTJobMatch.Services
{
    public interface IResponseCacheService
    {
        // Method to asynchronously set a cached response
        Task SetCacheResponseAsync(string cacheKey, object response, TimeSpan timeOut);

        // Method to asynchronously get a cached response
        Task<string> GetCacheResponseAsync(string cacheKey);

        // Method to asynchronously remove cached responses based on a pattern
        Task RemoveCacheResponseAsync(string pattern);
    }
}
