using System;
using System.Collections.Generic;
using System.Linq;
using Castle.DynamicProxy;

namespace AspectCache.Tests
{
    public class DummyClass : IDummyInterface
    {
        public List<String> MethodsCalled = new List<string>();

        public virtual DateTime UnCachedMethod()
        {
            MethodsCalled.Add("UnCachedMethod");
            return DateTime.Now;
        }

        [Cached]
        public virtual DateTime CachedMethod()
        {
            MethodsCalled.Add("CachedMethod");
            return DateTime.Now;
        }

        [Cached(TimeoutInMinutes = 5, CacheKeyGenerator = typeof(DummyCacheKeyGenerator), CacheRegion = "region")]
        public virtual DateTime CachedMethodWithSettings(string argumentOne, int argumentTwo)
        {
            MethodsCalled.Add("CachedMethodWithSettings");
            return DateTime.Now;
        }

        [Cached]
        public DateTime CachedInterfaceMethod()
        {
            MethodsCalled.Add("CachedInterfaceMethod");
            return DateTime.Now;
        }

        [BustCache]
        public void BustClassCache()
        {
            MethodsCalled.Add("BustClassCache");
        }

        [BustCache(MethodName = "CachedMethod")]
        public void BustMethodCache()
        {
            MethodsCalled.Add("BustMethodCache");
        }
    }

    public interface IDummyInterface
    {
        DateTime CachedInterfaceMethod();
    }

    public class UnCachedDummyClass
    {
        public DateTime UnAttributedMethod()
        {
            return DateTime.Now;
        }
    }

    public class DummyCacheProvider : ICacheProvider
    {
        public List<DummyCacheItem> Items = new List<DummyCacheItem>();

        public bool Contains(string cacheKey, string cacheRegion = null)
        {
            return Items.Any(i => i.CacheKey == cacheKey && i.CacheRegion == cacheRegion);
        }

        public object Get(string cacheKey, string cacheRegion = null)
        {
            return Items.First(i => i.CacheKey == cacheKey && i.CacheRegion == cacheRegion).Value;
        }

        public void Add(string cacheKey, object value, DateTimeOffset absoluteExpiration, string cacheRegion = null)
        {
            Items.Add(new DummyCacheItem
            {
                AbsoluteExpiration = absoluteExpiration,
                CacheKey = cacheKey,
                CacheRegion = cacheRegion,
                Value = value
            });
        }
        
        public void Remove(string cacheKey, string cacheRegion = null)
        {
            var item = Items.First(i => i.CacheKey == cacheKey && i.CacheRegion == cacheRegion);
            Items.Remove(item);
        }

        public void RemoveAllStartingWith(string cacheKey, string cacheRegion = null)
        {
            Items.RemoveAll(i => i.CacheKey.StartsWith(cacheKey) && i.CacheRegion == cacheRegion);
        }

        public class DummyCacheItem
        {
            public string CacheKey;
            public string CacheRegion;
            public object Value;
            public DateTimeOffset AbsoluteExpiration;
        }
    }

    public class DummyCacheKeyGenerator : ICacheKeyGenerator
    {
        private readonly string _cacheKey;

        public DummyCacheKeyGenerator(string cacheKey)
        {
            _cacheKey = cacheKey;
        }

        public string GenerateCacheKey(System.Reflection.MethodInfo method, object[] arguments)
        {
            return _cacheKey;
        }

        public string GeneratePartialCacheKey(System.Reflection.MethodInfo method)
        {
            throw new NotImplementedException();
        }
    }
}
