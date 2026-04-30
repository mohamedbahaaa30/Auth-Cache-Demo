using AuthDemo.Services.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace AuthDemo.Services
{
    public class CacheService : ICacheService
    {
        private IDistributedCache _distributedCache;
        public CacheService(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
        }
        public async Task<T> GetDataAsync<T>(string key)
        {
            var data = await _distributedCache.GetStringAsync(key);
            if (string.IsNullOrEmpty(data))
                return default(T);
            
            return JsonSerializer.Deserialize<T>(data);
        }

        public async Task<bool> RemoveDataAsync(string key)
        {
            var data = await _distributedCache.GetStringAsync(key);
            if (string.IsNullOrEmpty(data))
                return false;

            await _distributedCache.RemoveAsync(key);
            return true;    
        }

        public async Task<bool> SetDataAsync<T>(string key, T value, DateTimeOffset expiration)
        {
            var expireTime = expiration.Subtract(DateTimeOffset.UtcNow);
            DistributedCacheEntryOptions options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expireTime
            };
            await _distributedCache.SetStringAsync(key, JsonSerializer.Serialize(value), options);
            return true;
        }
    }
}
