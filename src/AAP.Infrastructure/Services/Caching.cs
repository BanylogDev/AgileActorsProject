using Microsoft.Extensions.Caching.Memory;

namespace AAP.Infrastructure.Services
{
    public class Caching
    {
        private readonly IMemoryCache _cache;

        public Caching(IMemoryCache cache)
        {
            _cache = cache;
        }

        public T? Get<T>(string key)
        {
            if (_cache.TryGetValue(key, out T? value))
                return value;

            return default;
        }

        public void Set<T>(string key, T value, int expirationMinutes = 5)
        {
            var options = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(expirationMinutes)
            };

            _cache.Set(key, value, options);
        }
    }
}