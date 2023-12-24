using System;
using System.Collections.Generic;
using System.Linq;
using Secyud.Ugf.ModManagement;
using Secyud.Ugf.Modularity.Plugins;
using Steamworks;

namespace Secyud.Ugf
{
    public class SteamPlugInSource : IPlugInSource
    {
        public List<SteamModInfo> SteamModInfos { get; } = new();


        public SteamPlugInSource()
        {
            uint itemNum = SteamUGC.GetNumSubscribedItems();
            PublishedFileId_t[] fileIds = new PublishedFileId_t[itemNum];
            SteamUGC.GetSubscribedItems(fileIds, itemNum);

            foreach (PublishedFileId_t fileId in fileIds)
            {
                try
                {
                    SteamModInfo steamModInfo = new(fileId);
                    steamModInfo.Refresh();
                    if (steamModInfo.Local is not null)
                    {
                        SteamModInfo existMod = SteamModInfos.FirstOrDefault(u =>
                            u.Local.ModId == steamModInfo.Local.ModId);
                        if (existMod is not null)
                        {
                            int index = SteamModInfos.IndexOf(existMod);
                            SteamModInfos[index] = steamModInfo;
                        }
                        else
                        {
                            SteamModInfos.Add(steamModInfo);
                        }
                    }
                }
                catch (Exception e)
                {
                    U.LogError($"Mod load failed: fileId-{fileId}");
                    U.LogError(e);
                }
            }
        }

        public IEnumerable<Type> GetModules()
        {
            return SteamModInfos
                .Where(u => u.ModuleType is not null)
                .Select(u => u.ModuleType);
        }
    }
}