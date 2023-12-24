using System.Linq;
using Secyud.Ugf.BasicComponents;
using Secyud.Ugf.TabComponents;
using Secyud.Ugf.TableComponents;
using UnityEngine;

namespace Secyud.Ugf.ModManagement
{
    public class SteamModLocalTab : TabPanel
    {
        [SerializeField] private Table Table;
        [SerializeField] private SText EnsureText;

        private SteamModInfo _selectedMod;

        private SteamModManageTabService _service;
        protected override TabService Service => _service;

        protected override void Awake()
        {
            _service = SteamModManageScope.Instance.Get<SteamModManageTabService>();
            base.Awake();
            RefreshTab();
        }

        public override void RefreshTab()
        {
            Table.AutoSetSingleSelectTable
                <SteamModInfo, SteamModInfoSorters, SteamModInfoFilters>(
                    SteamManager.Instance.PlugInSource.SteamModInfos
                        .Where(u => u.Local is not null).ToList(),
                    EnableOrDisableMod,SetCell);
        }

        private void SetCell(TableCell cell,SteamModInfo info)
        {
            cell.BindShowable(info);
            SetEnsureText(info.Local.Disabled);
        }
        
        private void EnableOrDisableMod(SteamModInfo info)
        {
            info.Local.Disabled = !info.Local.Disabled;
            info.Local.WriteToContent(info.Folder);
            SetEnsureText(info.Local.Disabled);
        }

        private void SetEnsureText(bool state)
        {
            EnsureText.text = state ? U.T["Enable"] : U.T["Disable"];
        }
    }
}