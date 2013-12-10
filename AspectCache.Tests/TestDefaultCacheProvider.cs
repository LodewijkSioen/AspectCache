using System;
using System.Runtime.Caching;
using NUnit.Framework;

namespace AspectCache.Tests
{
    [TestFixture]
    public class TestDefaultCacheProvider
    {
        [Test]
        public void TestDefaultMemoryCacheUsage()
        {
            var defaultCache = new DefaultCacheProvider();

            defaultCache.Add("test", "value", DateTimeOffset.MaxValue, "region");

            AssertCacheUsage(defaultCache);
            Assert.That(MemoryCache.Default.Get("[region]test"), Is.EqualTo("value"));
            
            CleanDefaultCache();

            var specificCache = new DefaultCacheProvider(new MemoryCache("Test"));

            specificCache.Add("test", "value", DateTimeOffset.MaxValue, "region");

            AssertCacheUsage(specificCache);
            Assert.That(MemoryCache.Default.Contains("[region]test"), Is.False);
        }

        private void CleanDefaultCache()
        {
            foreach (var cacheItem in MemoryCache.Default)
            {
                MemoryCache.Default.Remove(cacheItem.Key);
            }
        }


        private static void AssertCacheUsage(ICacheProvider cache)
        {
            Assert.That(cache.Contains("test", "region"), Is.True);
            Assert.That(cache.Get("test", "region"), Is.EqualTo("value"));
        }
    }
}