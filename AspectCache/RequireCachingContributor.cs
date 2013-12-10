using System;
using System.Linq;
using Castle.Core;
using Castle.Core.Internal;
using Castle.MicroKernel;
using Castle.MicroKernel.ModelBuilder;

namespace AspectCache
{
    public class RequireCachingContributor : IContributeComponentModelConstruction
    {
        public void ProcessModel(IKernel kernel, ComponentModel model)
        {
            var cachedMethods = model.Implementation.GetMethods().Where(m => AttributesUtil.GetAttribute<CachedAttribute>(m) != null).ToList();

            if (cachedMethods.Any())
            {
#if DEBUG
                foreach (var cachedMethod in cachedMethods)
                {
                    if (!cachedMethod.IsVirtual)
                    {
                        throw new NotImplementedException(String.Format("Method {0} on class {1} has a CachedAttribute, but is not a virtual method. For caching to work, this method must be virtual.", 
                            cachedMethod.Name, cachedMethod.DeclaringType));
                    }
                }
#endif
                model.Interceptors.AddIfNotInCollection(InterceptorReference.ForType<CacheInterceptor>());
            }
        }
    }
}