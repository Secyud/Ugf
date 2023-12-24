using System;
using Secyud.Ugf.AssetComponents;
using Secyud.Ugf.DependencyInjection;

namespace Secyud.Ugf.ModManagement
{
    public sealed class SteamModManageScope : DependencyScopeProvider
    {
        public static SteamModManageScope Instance { get; private set; }
        public static MonoContainer<SteamModManagePanel> ModManagePanel { get; set; }

        public SteamModManageScope()
        {
            ModManagePanel = MonoContainer<SteamModManagePanel>.Create(
                U.Tm[new Guid("11E30167-E0BD-61B9-8F41-46776AF49A11")].Type);
        }

        public override void OnInitialize()
        {
            Instance = this;
            ModManagePanel?.Create();
        }

        public override void Dispose()
        {
            ModManagePanel?.Destroy();
            Instance = null;
        }
    }
}