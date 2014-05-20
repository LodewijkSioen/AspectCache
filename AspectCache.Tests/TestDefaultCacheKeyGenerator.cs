using System;
using System.Reflection;
using Castle.DynamicProxy;
using NUnit.Framework;

namespace AspectCache.Tests
{
    [TestFixture]
    public class TestDefaultCacheKeyGenerator
    {
        [Test]
        public void ACacheKeyIsGeneratedFromTheMethodSignature()
        {
            var generator = new DefaultCacheKeyGenerator();

            var type = typeof (DummyClass);
            var method = type.GetMethod("CachedMethodWithSettings");
            var key = generator.GenerateCacheKey(method, new object[]{"test", 1});

            Assert.That(key, Is.EqualTo("AspectCache.Tests.DummyClass.CachedMethodWithSettings(test, 1)"));
        }

        [Test]
        public void APartialCacheKeyIsGeneratedFromTheMethodSignature()
        {
            var generator = new DefaultCacheKeyGenerator();

            var type = typeof(DummyClass);
            var method = type.GetMethod("CachedMethodWithSettings");
            var key = generator.GeneratePartialCacheKey(method);

            Assert.That(key, Is.EqualTo("AspectCache.Tests.DummyClass.CachedMethodWithSettings"));
        }
    }
}