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
            var cachedMethods = model.Implementation.GetMethods().Where(m => m.GetAttribute<CachedAttribute>() != null).ToList();

            if (cachedMethods.Any())
            {
                cachedMethods.CheckThatMethodsAreVirtual();
                model.Interceptors.AddIfNotInCollection(InterceptorReference.ForType<CacheInterceptor>());
            }
        }
    }
}