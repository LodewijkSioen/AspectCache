using System;
using System.Runtime.Caching;
using NUnit.Framework;

namespace AspectCache.Tests
{
    [TestFixture]
    public class TestDefaultCacheProvider
    {
        ICacheProvider _provider;
        MemoryCache _store;

        [SetUp]
        public void Setup()
        {
            _store = new MemoryCache("Test");
            _provider = new DefaultCacheProvider(_store);
        }

        [TearDown]
        public void Teardown()
        {
            MemoryCache.Default.Remove("test");
            _store.Dispose();
        }

        [Test]
        public void TestUsage()
        {
            _provider.Add("test", "value", DateTimeOffset.MaxValue, "region");

            Assert.That(_provider.Contains("test", "region"), Is.True);
            Assert.That(_provider.Get("test", "region"), Is.EqualTo("value"));
        }

        [Test]
        public void TestRemoveFromCache()
        {
            _provider.Add("test", "value", DateTimeOffset.MaxValue, "region");
            Assert.That(_store.Get("[region]test"), Is.EqualTo("value"));

            _provider.Remove("test", "region");
            Assert.That(_store.Contains("[region]test"), Is.False);
        }

        [Test]
        public void TestRemoveStartingWith()
        {
            _provider.Add("test", "value", DateTimeOffset.MaxValue, "region");
            Assert.That(_store.Get("[region]test"), Is.EqualTo("value"));

            _provider.RemoveAllStartingWith("t", "region");
            Assert.That(_store.Contains("[region]test"), Is.False);
        }

        [Test]
        public void TestDefaultConstructor()
        {
            var provider = new DefaultCacheProvider();
            provider.Add("test", "value", DateTimeOffset.MaxValue);
            Assert.That(MemoryCache.Default["test"], Is.EqualTo("value"));
        }

        [Test]
        public void TestAddNullWillRemoveFromCache()
        {
            var provider = new DefaultCacheProvider();
            provider.Add("test", "value", DateTimeOffset.MaxValue);
            Assert.That(MemoryCache.Default["test"], Is.EqualTo("value"));

            provider.Add("test", null, DateTimeOffset.MaxValue);
            Assert.That(MemoryCache.Default["test"], Is.EqualTo(null));
        }
    }
}