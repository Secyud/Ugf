#region

using Secyud.Ugf.Modularity;

#endregion

namespace Secyud.Ugf
{
    [DependsOn(
        typeof(UgfCoreModule),
        typeof(UgfUnityModule)
    )]
    public class UgfUnityUiExModule : IUgfModule
    {
        public void Configure(ConfigurationContext context)
        {
            context.Manager.AddAssembly(typeof(UgfUnityUiExModule).Assembly);
        }
    }
}