using System;
using System.Reflection;

namespace AspectCache
{
    public class DefaultCacheKeyGenerator : ICacheKeyGenerator
    {
        public string GenerateCacheKey(MethodInfo method, object[] arguments = null)
        {
            return String.Concat(method.DeclaringType.FullName, ".", method.Name, "(", String.Join(", ", arguments), ")");
        }

        public string GeneratePartialCacheKey(MethodInfo method)
        {
            return String.Concat(method.DeclaringType.FullName, ".", method.Name);
        }
    }
}