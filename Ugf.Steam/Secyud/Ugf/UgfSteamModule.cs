using Secyud.Ugf.Modularity;
using Secyud.Ugf.UpdateComponents;
using Steamworks;

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
            if (!U.DataManager)
            {
                context.Manager.RegisterInstance(SteamManager.Instance);

                IUpdateService updateService = context.Get<IUpdateService>();
                updateService.UpdateAction += SteamAPI.RunCallbacks;
            }
        }
    }
}