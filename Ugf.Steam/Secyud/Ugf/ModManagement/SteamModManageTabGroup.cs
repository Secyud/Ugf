using Secyud.Ugf.TabComponents;

namespace Secyud.Ugf.ModManagement
{
    public class SteamModManageTabGroup:TabGroup
    {
        protected override TabService Service => SteamModManageScope.Instance.Get<SteamModManageTabService>();

    }
}