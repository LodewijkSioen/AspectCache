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

            var method2 = type.GetMethod("CachedMethod");
            var key2 = generator.GenerateCacheKey(method2);
            Assert.That(key2, Is.EqualTo("AspectCache.Tests.DummyClass.CachedMethod()"));
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

        [Test]
        public void APartialCacheKeyIsGeneratedFromTheTypeSignature()
        {
            var generator = new DefaultCacheKeyGenerator();

            var type = typeof(DummyClass);
            var key = generator.GeneratePartialCacheKey(type);

            Assert.That(key, Is.EqualTo("AspectCache.Tests.DummyClass"));
        }
    }
}