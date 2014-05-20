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
        }

        public void BustCache(BustCacheAttribute attribute, IInvocation invocation)
        {
            var cacheKeyGenerator = _cacheKeyGenerators.FirstOrDefault(c => c.GetType() == attribute.CacheKeyGenerator) ?? new DefaultCacheKeyGenerator();

            var method = invocation.TargetType.GetMethod(attribute.MethodName ?? string.Empty);

            if (method == null)
            {
                var cacheKey = cacheKeyGenerator.GeneratePartialCacheKey(invocation.TargetType);
                _cache.RemoveAllStartingWith(cacheKey, attribute.CacheRegion);
            }
            else if (method.GetParameters().Any())
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
}