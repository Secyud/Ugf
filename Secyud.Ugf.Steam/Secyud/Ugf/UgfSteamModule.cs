using System.IO;
using Secyud.Ugf.Modularity;
using Secyud.Ugf.Steam.WorkshopManager;
using Secyud.Ugf.VirtualPath;

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

            IVirtualPathManager virtualPathManager = context.Get<IVirtualPathManager>();
            foreach (WorkshopItemInfo info in LocalWorkshopManager.LocalItems)
            {
                if (info?.ConfigInfo?.MapFolders is null) continue;
                foreach (string folder in info.ConfigInfo.MapFolders)
                {
                    virtualPathManager.AddDirectory(folder,
                        Path.Combine(info.LocalPath, folder));
                }
            }
        }
    }
}