using System.Diagnostics;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Secyud.Ugf.ModManagement
{
    public class SteamModManagePanel : MonoBehaviour
    {
        public void Close()
        {
            U.M.DestroyScope<SteamModManageScope>();
        }

        public void EnsureSetting()
        {
            {
                var saveMod = SteamManager.Instance.PlugInSource.SteamModInfos
                    .Where(u => u.Local?.Disabled == false).ToList();
                var path = Path.Combine(U.Path, "steam-mod-info.binary");
                using FileStream stream = File.OpenWrite(path);
                using BinaryWriter writer = new(stream);
                writer.Write(saveMod.Count);

                foreach (SteamModInfo info in saveMod)
                {
                    writer.Write(info.Folder);
                }
            }


            Application.Quit();
            Process.Start(Process.GetCurrentProcess().MainModule!.FileName);
        }
    }
}