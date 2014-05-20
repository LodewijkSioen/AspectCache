using System;
using System.Collections.Generic;
using System.Linq;

namespace AspectCache.Tests
{
    public static class Randomizer
    {
        static readonly Random Random = new Random();

        public static int GetValue()
        {
            return Random.Next();
        }
    }

    public class DummyClass
    {
        public List<String> MethodsCalled = new List<string>();

        public virtual int UnCachedMethod()
        {
            MethodsCalled.Add("UnCachedMethod");
            return Randomizer.GetValue();
        }

        [Cached]
        public virtual int CachedMethod()
        {
            MethodsCalled.Add("CachedMethod");
            return Randomizer.GetValue();
        }

        [Cached(CacheRegion = "region")]
        public virtual int CachedMethodWithRegion()
        {
            MethodsCalled.Add("CachedMethodWithRegion");
            return Randomizer.GetValue();
        }

        [Cached(TimeoutInMinutes = 5, CacheKeyGenerator = typeof(DummyCacheKeyGenerator), CacheRegion = "region")]
        public virtual int CachedMethodWithSettings(string argumentOne, int argumentTwo)
        {
            MethodsCalled.Add("CachedMethodWithSettings");
            return Randomizer.GetValue();
        }

        [BustCache(CacheRegion = "region")]
        public virtual void BustClassCache()
        {
            MethodsCalled.Add("BustClassCache");
        }

        [BustCache(MethodName = "CachedMethod")]
        public virtual void BustMethodCache()
        {
            MethodsCalled.Add("BustMethodCache");
        }
    }

    public interface IDummyInterface
    {
        int CachedInterfaceMethod();
    }

    public class CrashingDummyClass
    {
        [Cached]
        public int CachedMethod()
        {
            return Randomizer.GetValue();
        }
    }

    public class CrashingDummyClassWithInterface : IDummyInterface
    {
        [Cached]
        public int CachedInterfaceMethod()
        {
            return Randomizer.GetValue();
        }
    }

    public class NonCrashingDummyClassWithInterface : IDummyInterface
    {
        [Cached]
        public virtual int CachedInterfaceMethod()
        {
            return Randomizer.GetValue();
        }
    }

    public class UnCachedDummyClass
    {
        public int UnAttributedMethod()
        {
            return Randomizer.GetValue();
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
            return _cacheKey;
        }

        public string GeneratePartialCacheKey(Type type)
        {
            return _cacheKey;
        }
    }
}
