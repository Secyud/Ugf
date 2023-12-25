using System.Diagnostics;
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
            Application.Quit();
            Process.Start(Process.GetCurrentProcess().MainModule!.FileName);
        }
    }
}