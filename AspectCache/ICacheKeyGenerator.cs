using System.Reflection;

namespace AspectCache
{
    public interface ICacheKeyGenerator
    {
        string GenerateCacheKey(MethodInfo method, object[] arguments = null);
        string GeneratePartialCacheKey(MethodInfo method);
    }
}