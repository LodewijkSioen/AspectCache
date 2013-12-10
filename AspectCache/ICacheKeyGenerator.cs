using Castle.DynamicProxy;

namespace AspectCache
{
    public interface ICacheKeyGenerator
    {
        string GenerateCacheKey(IInvocation invocation);
    }
}