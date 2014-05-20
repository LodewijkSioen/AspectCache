using System;

namespace AspectCache
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class BustCacheAttribute : Attribute
    {
        public string MethodName { get; set; }
        public string CacheRegion { get; set; }
        public Type CacheKeyGenerator { get; set; }
    }
}