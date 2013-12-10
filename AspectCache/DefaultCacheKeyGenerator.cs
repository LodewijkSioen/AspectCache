using System;
using Castle.DynamicProxy;

namespace AspectCache
{
    public class DefaultCacheKeyGenerator : ICacheKeyGenerator
    {
        public string GenerateCacheKey(IInvocation invocation)
        {
            return String.Concat(invocation.TargetType.FullName, ".", invocation.Method.Name, "(", String.Join(", ", invocation.Arguments), ")");
        }
    }
}