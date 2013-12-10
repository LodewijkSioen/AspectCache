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
            var key = generator.GenerateCacheKey(new DummyInvocation(type, method, new object[]{"test", 1}));

            Assert.That(key, Is.EqualTo("AspectCache.Tests.DummyClass.CachedMethodWithSettings(test, 1)"));
        }

        class DummyInvocation : IInvocation
        {
            public DummyInvocation(Type targetType, MethodInfo method, object[] arguments)
            {
                TargetType = targetType;
                Method = method;
                Arguments = arguments;
            }

            public object GetArgumentValue(int index)
            {
                throw new NotImplementedException();
            }

            public MethodInfo GetConcreteMethod()
            {
                throw new NotImplementedException();
            }

            public MethodInfo GetConcreteMethodInvocationTarget()
            {
                throw new NotImplementedException();
            }

            public void Proceed()
            {
                throw new NotImplementedException();
            }

            public void SetArgumentValue(int index, object value)
            {
                throw new NotImplementedException();
            }

            public object[] Arguments { get; private set; }
            public Type[] GenericArguments { get; private set; }
            public object InvocationTarget { get; private set; }
            public MethodInfo Method { get; private set; }
            public MethodInfo MethodInvocationTarget { get; private set; }
            public object Proxy { get; private set; }
            public object ReturnValue { get; set; }
            public Type TargetType { get; private set; }
        }
    }
}