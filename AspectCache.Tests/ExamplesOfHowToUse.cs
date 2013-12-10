using System.Runtime.Caching;
using System.Threading;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using NUnit.Framework;

namespace AspectCache.Tests
{
    [TestFixture]
    public class ExamplesOfHowToUse
    {
        [Test]
        public void NonAttributedMethodsAreUnCached()
        {
            using (var container = CreateContainer())
            {
                var dummy = container.Resolve<DummyClass>();

                var one = dummy.UnCachedMethod();
                Thread.Sleep(1);
                var two = dummy.UnCachedMethod();

                Assert.That(one, Is.Not.EqualTo(two));
                Assert.That(dummy.MethodsCalled.Count, Is.EqualTo(2));
            }
        }

        [Test]
        public void AttributedVirtualMethodsAreCached()
        {
            using (var container = CreateContainer())
            {
                var dummy = container.Resolve<DummyClass>();

                var one = dummy.CachedMethod();
                Thread.Sleep(1);
                var two = dummy.CachedMethod();

                Assert.AreEqual(one, two);
                Assert.That(dummy.MethodsCalled.Count, Is.EqualTo(1));
            }
        }

        [Test]
        public void AttributedNonVirtualMethodsEvenIfTheyComeFromAnInterfaceAreNotCached()
        {
            using (var container = CreateContainer())
            {
                var dummy = container.Resolve<DummyClass>();

                var one = dummy.CachedInterfaceMethod();
                Thread.Sleep(1);
                var two = dummy.CachedInterfaceMethod();

                Assert.AreNotEqual(one, two);
                Assert.That(dummy.MethodsCalled.Count, Is.EqualTo(2));
            }
        }

        [Test]
        public void HowToOverrideTheCacheProvider()
        {
            using (var container = CreateContainer())
            {
                container.Register(
                    Component.For<ICacheProvider>().ImplementedBy<DummyCacheProvider>().IsDefault(),
                    Component.For<ICacheKeyGenerator>().Instance(new DummyCacheKeyGenerator("test"))
                );

                var dummy = container.Resolve<DummyClass>();
                var value = dummy.CachedMethodWithSettings("test", 2);

                //The new implementations of CacheProvider and CacheKeyGenerator are used
                var cache = container.Resolve<ICacheProvider>();
                Assert.That(cache, Is.TypeOf<DummyCacheProvider>());
                Assert.That(cache.Contains("test", "region"), Is.True);
                Assert.That(cache.Get("test", "region"), Is.EqualTo(value));
            }
        }

        private static IWindsorContainer CreateContainer()
        {
            var container = new WindsorContainer();
            container.AddFacility<CacheFacility>();
            container.Register(
                Component.For<DummyClass>()
            );

            return container;
        }

        [SetUp]
        public void Setup()
        {
            foreach (var item in MemoryCache.Default)
            {
                MemoryCache.Default.Remove(item.Key);
            }
            Assert.That(MemoryCache.Default.GetCount(), Is.EqualTo(0));
        }
    }
}