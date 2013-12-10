using System;
using System.Collections.ObjectModel;
using System.Linq;
using Castle.Core;
using NUnit.Framework;

namespace AspectCache.Tests
{
    [TestFixture]
    public class TestRequireCachingContributor
    {
        [Test]
        public void AClassWithAttributedMethodsWillGetTheInterceptor()
        {
            var contributor = new RequireCachingContributor();
            var dummyModel = new ComponentModel(new ComponentName("DummyClass", false), new Collection<Type>{typeof(DummyClass)}, typeof(DummyClass), null);
            Assert.That(dummyModel.HasInterceptors, Is.False);

            contributor.ProcessModel(null, dummyModel);

            Assert.That(dummyModel.Interceptors.Count, Is.EqualTo(1));
            Assert.That(dummyModel.Interceptors.ElementAt(0), Is.EqualTo(InterceptorReference.ForType<CacheInterceptor>()));
        }

        [Test]
        public void AClassWithoutAttributedMethodsWillNotGetTheInterceptor()
        {
            var contributor = new RequireCachingContributor();
            var dummyModel = new ComponentModel(new ComponentName("UnCachedDummyClass", false), new Collection<Type> { typeof(UnCachedDummyClass) }, typeof(UnCachedDummyClass), null);
            Assert.That(dummyModel.HasInterceptors, Is.False);

            contributor.ProcessModel(null, dummyModel);

            Assert.That(dummyModel.HasInterceptors, Is.False);
        }
    }
}