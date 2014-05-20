using System;
using System.Collections.Generic;
using System.Reflection;

namespace AspectCache
{
    public static class Extensions
    {
        public static void CheckThatMethodsAreVirtual(this IEnumerable<MethodInfo> methods)
        {
            foreach (var method in methods)
            {
                if (!(method.IsVirtual && !method.IsFinal))
                {
                    throw new NotImplementedException(String.Format("Method '{0}' on class '{1}' is not a virtual method. For caching to work, this method must be virtual.",
                        method.Name, method.DeclaringType));
                }
            }
        }
    }
}