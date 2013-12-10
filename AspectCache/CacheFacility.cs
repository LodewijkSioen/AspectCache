using Castle.Core.Configuration;
using Castle.DynamicProxy;
using Castle.MicroKernel;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.Resolvers.SpecializedResolvers;

namespace AspectCache
{
    public class CacheFacility : IFacility
    {
        public void Init(IKernel kernel, IConfiguration facilityConfig)
        {
            kernel.Resolver.AddSubResolver(new CollectionResolver(kernel));
            kernel.ComponentModelBuilder.AddContributor(new RequireCachingContributor());
            kernel.Register(
                Component.For<ICacheProvider>().ImplementedBy<DefaultCacheProvider>(),
                Component.For<IInterceptor>().ImplementedBy<CacheInterceptor>(),
                Component.For<ICacheKeyGenerator>().ImplementedBy<DefaultCacheKeyGenerator>()
            );
        }

        public void Terminate()
        {
            
        }
    }
}