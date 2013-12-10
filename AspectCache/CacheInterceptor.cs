using System;
using System.Collections.Generic;
using System.Linq;
using Castle.Core.Internal;
using Castle.DynamicProxy;

namespace AspectCache
{
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

            var cacheKeyGenerator = _cacheKeyGenerators.FirstOrDefault(c => c.GetType() == cacheAttribute.CacheKeyGenerator) ?? new DefaultCacheKeyGenerator();
            var cacheKey = cacheKeyGenerator.GenerateCacheKey(invocation);

            if (_cache.Contains(cacheKey, cacheAttribute.CacheRegion))
            {
                invocation.ReturnValue = _cache.Get(cacheKey, cacheAttribute.CacheRegion);
            }
            else
            {
                invocation.Proceed();
                var expiration = cacheAttribute.TimeoutInMinutes == 0 ? DateTimeOffset.MaxValue : DateTimeOffset.Now.AddMinutes(cacheAttribute.TimeoutInMinutes);
                _cache.Add(cacheKey, invocation.ReturnValue, expiration, cacheAttribute.CacheRegion);
            }
        }
    }
}