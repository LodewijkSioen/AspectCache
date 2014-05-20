using System;
using System.Collections.Generic;
using System.Linq;
using Castle.Core.Internal;
using Castle.DynamicProxy;

namespace AspectCache
{
    public class BustCacheInterceptor : IInterceptor
    {
        private readonly IEnumerable<ICacheKeyGenerator> _cacheKeyGenerators;
        private readonly ICacheProvider _cache;

        public BustCacheInterceptor(ICacheProvider cache, IEnumerable<ICacheKeyGenerator> cacheKeyGenerators)
        {
            _cache = cache;
            _cacheKeyGenerators = cacheKeyGenerators;
        }

        public void Intercept(IInvocation invocation)
        {
            var bustCacheAttribute = invocation.Method.GetAttribute<BustCacheAttribute>();

            if (bustCacheAttribute == null)
            {
                invocation.Proceed();
                return;
            }

            invocation.Proceed();
            BustCache(bustCacheAttribute, invocation);
            return;
        }

        public void BustCache(BustCacheAttribute attribute, IInvocation invocation)
        {
            var cacheKeyGenerator = _cacheKeyGenerators.FirstOrDefault(c => c.GetType() == attribute.CacheKeyGenerator) ?? new DefaultCacheKeyGenerator();

            var method = invocation.TargetType.GetMethod(attribute.MethodName);

            if (method.GetParameters().Any())
            {
                var cacheKey = cacheKeyGenerator.GeneratePartialCacheKey(method);
                _cache.RemoveAllStartingWith(cacheKey, attribute.CacheRegion);
            }
            else
            {
                var cacheKey = cacheKeyGenerator.GenerateCacheKey(method);
                _cache.Remove(cacheKey, attribute.CacheRegion);
            }
        }
    }

    public class CacheInterceptor : IInterceptor
    {
        private readonly IEnumerable<ICacheKeyGenerator> _cacheKeyGenerators;
        private readonly ICacheProvider _cache;

        public CacheInterceptor(ICacheProvider cache, IEnumerable<ICacheKeyGenerator> cacheKeyGenerators)
        {
            _cache = cache;
            _cacheKeyGenerators = cacheKeyGenerators;
        }

        public void Intercept(IInvocation invocation)
        {
            var cacheAttribute = invocation.Method.GetAttribute<CachedAttribute>();

            if (cacheAttribute == null)
            {
                invocation.Proceed();
                return;
            }

            FillCache(cacheAttribute, invocation);
            return;
        }        

        public void FillCache(CachedAttribute attribute, IInvocation invocation)
        {
            var cacheKeyGenerator = _cacheKeyGenerators.FirstOrDefault(c => c.GetType() == attribute.CacheKeyGenerator) ?? new DefaultCacheKeyGenerator();
            var cacheKey = cacheKeyGenerator.GenerateCacheKey(invocation.Method, invocation.Arguments);

            if (_cache.Contains(cacheKey, attribute.CacheRegion))
            {
                invocation.ReturnValue = _cache.Get(cacheKey, attribute.CacheRegion);
            }
            else
            {
                invocation.Proceed();
                var expiration = attribute.TimeoutInMinutes == 0 ? DateTimeOffset.MaxValue : DateTimeOffset.Now.AddMinutes(attribute.TimeoutInMinutes);
                _cache.Add(cacheKey, invocation.ReturnValue, expiration, attribute.CacheRegion);
            }
        }
    }
}