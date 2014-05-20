using System;
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
        public void BustTheCacheForTheEntireClass()
        {
            using (var container = CreateContainer())
            {
                var dummy = container.Resolve<DummyClass>();
                var one = dummy.CachedMethodWithRegion();
                var two = dummy.CachedMethodWithSettings("one", 2);

                Assert.That(dummy.CachedMethodWithRegion(), Is.EqualTo(one));
                Assert.That(dummy.CachedMethodWithSettings("one", 2), Is.EqualTo(two));

                dummy.BustClassCache();

                Assert.That(dummy.CachedMethodWithRegion(), Is.Not.EqualTo(one));
                Assert.That(dummy.CachedMethodWithSettings("one", 2), Is.Not.EqualTo(two));
            }
        }

        [Test]
        public void BustTheCacheForSpecificMethod()
        {
            using (var container = CreateContainer())
            {
                var dummy = container.Resolve<DummyClass>();
                var one = dummy.CachedMethod();

                Assert.That(dummy.CachedMethod(), Is.EqualTo(one));

                dummy.BustMethodCache();

                Assert.That(dummy.CachedMethod(), Is.Not.EqualTo(one));
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

        [Test]
        public void MethodsWithAttributesMustBeVirtualEvenForInterfaces()
        {
            using (var container = CreateContainer())
            {
                var ex = Assert.Throws<NotImplementedException>(() => container.Register(Component.For<CrashingDummyClassWithInterface>()));
                Assert.That(ex.Message, Is.EqualTo("Method 'CachedInterfaceMethod' on class 'AspectCache.Tests.CrashingDummyClassWithInterface' is not a virtual method. For caching to work, this method must be virtual."));

                container.Register(Component.For<NonCrashingDummyClassWithInterface>());
                var dummy = container.Resolve<NonCrashingDummyClassWithInterface>();
                var one = dummy.CachedInterfaceMethod();
                var two = dummy.CachedInterfaceMethod();
                Assert.That(one, Is.EqualTo(two));
            }
        }

        [Test]
        public void MethodsWithAttributesMustBeVirtual()
        {
            using (var container = CreateContainer())
            {
                var ex = Assert.Throws<NotImplementedException>(() => container.Register(Component.For<CrashingDummyClass>()));
                Assert.That(ex.Message, Is.EqualTo("Method 'CachedMethod' on class 'AspectCache.Tests.CrashingDummyClass' is not a virtual method. For caching to work, this method must be virtual."));
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