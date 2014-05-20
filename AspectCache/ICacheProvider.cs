using System;

namespace AspectCache
{
    public interface ICacheProvider
    {
        Boolean Contains(string cacheKey, string cacheRegion = null);
        object Get(string cacheKey, string cacheRegion = null);
        void Add(string cacheKey, object value, DateTimeOffset absoluteExpiration, string cacheRegion = null);
        void Remove(string cacheKey, string cacheRegion = null);
        void RemoveAllStartingWith(string cacheKey, string cacheRegion = null);
    }
}