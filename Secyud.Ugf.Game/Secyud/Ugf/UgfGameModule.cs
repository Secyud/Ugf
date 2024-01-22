using Secyud.Ugf.Modularity;

namespace Secyud.Ugf
{
    [DependsOn(
        typeof(UgfUnityModule)
    )]
    public class UgfGameModule : IUgfModule
    {
        public void Configure(ConfigurationContext context)
        {
            context.Manager.AddAssembly(typeof(UgfGameModule).Assembly);
        }
    }
}