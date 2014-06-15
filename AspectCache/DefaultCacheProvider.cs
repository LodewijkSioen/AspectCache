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
            if (value == null)
            {
                _cache.Remove(ComposeCacheKey(cacheKey, cacheRegion));
            }
            else
            {
                _cache.Add(ComposeCacheKey(cacheKey, cacheRegion), value, absoluteExpiration);
            }
        }

        public void Remove(string cacheKey, string cacheRegion = null)
        {
            _cache.Remove(ComposeCacheKey(cacheKey, cacheRegion));
        }

        public void RemoveAllStartingWith(string cacheKey, string cacheRegion = null)
        {
            var compositeCacheKey = ComposeCacheKey(cacheKey, cacheRegion);
            foreach (var item in _cache)
            {
                if (item.Key.StartsWith(compositeCacheKey))
                {
                    _cache.Remove(item.Key);
                }
            }
        }
    }
}