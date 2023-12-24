using Secyud.Ugf.Modularity;

namespace Secyud.Ugf
{
    [DependsOn(
        typeof(UgfCoreModule),
        typeof(UgfUnityModule))]
    public class UgfSteamModule : IUgfModule
    {
        public void Configure(ConfigurationContext context)
        {
            context.Manager.AddAssembly(typeof(UgfSteamModule).Assembly);
            context.Manager.RegisterInstance(SteamManager.Instance);
        }
    }
}