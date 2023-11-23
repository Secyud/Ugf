#region

using Secyud.Ugf.Modularity;

#endregion

namespace Secyud.Ugf
{
    [DependsOn(
        typeof(UgfCoreModule),
        typeof(UgfUnityModule)
    )]
    public class UgfDotweenExModule : IUgfModule
    {
        public void Configure(ConfigurationContext context)
        {
            context.Manager.AddAssembly(typeof(UgfDotweenExModule).Assembly);
        }
    }
}