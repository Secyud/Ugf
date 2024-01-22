using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Secyud.Ugf.Steam.WorkshopManager
{
    public class WorkshopItemManageFunction : MonoBehaviour
    {
        public void SaveLocalItemMessageAndRestart()
        {
            {
                List<WorkshopItemInfo> saveItems = LocalWorkshopManager.LocalItems
                    .Where(u => u.ConfigInfo?.Disabled == false).ToList();
                string path = Path.Combine(U.Path, "steam-mod-info.binary");
                using FileStream stream = File.OpenWrite(path);
                using BinaryWriter writer = new(stream);
                writer.Write(saveItems.Count);

                foreach (WorkshopItemInfo info in saveItems)
                {
                    writer.Write(info.LocalPath);
                }
            }
            Application.Quit();
            Process.Start(Process.GetCurrentProcess().MainModule!.FileName);
        }
    }
}