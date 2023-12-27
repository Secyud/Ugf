using System.IO;
using Secyud.Ugf.ModManagement;
using Secyud.Ugf.Modularity;
using Secyud.Ugf.UpdateComponents;
using Secyud.Ugf.VirtualPath;
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
            context.Manager.RegisterInstance(SteamManager.Instance);
            IUpdateService updateService = context.Get<IUpdateService>();
            updateService.UpdateAction += SteamAPI.RunCallbacks;

            IVirtualPathManager virtualPathManager = context.Get<IVirtualPathManager>();
            foreach (SteamModInfo info in SteamManager.Instance.PlugInSource.SteamModInfos)
            {
                if (info?.Local?.MapFolders is null) continue;
                foreach (string folder in info.Local.MapFolders)
                {
                    virtualPathManager.AddDirectory(folder,
                        Path.Combine(info.Folder, folder));
                }
            }
        }
    }
}