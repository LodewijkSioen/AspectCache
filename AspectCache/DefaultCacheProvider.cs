using System;
using System.Runtime.Caching;

namespace AspectCache
{
    public class DefaultCacheProvider : ICacheProvider
    {
        private readonly ObjectCache _cache;

        public DefaultCacheProvider(ObjectCache cache = null)
        {
            _cache = cache ?? MemoryCache.Default;
        }

        /// <summary>
        /// The default MemoryCache doesn't implement regions, so we cheat here
        /// </summary>
        private string ComposeCacheKey(string cacheKey, string cacheRegion)
        {
            return cacheRegion == null ? cacheKey : string.Concat("[", cacheRegion, "]", cacheKey);
        }

        public bool Contains(string cacheKey, string cacheRegion = null)
        {
            return _cache.Contains(ComposeCacheKey(cacheKey, cacheRegion));
        }

        public object Get(string cacheKey, string cacheRegion = null)
        {
            return _cache.Get(ComposeCacheKey(cacheKey, cacheRegion));
        }

        public void Add(string cacheKey, object value, DateTimeOffset absoluteExpiration, string cacheRegion = null)
        {
            _cache.Add(ComposeCacheKey(cacheKey, cacheRegion), value, absoluteExpiration);
        }
    }
}