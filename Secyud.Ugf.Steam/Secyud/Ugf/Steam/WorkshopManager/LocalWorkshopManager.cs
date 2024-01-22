using System;
using System.Collections.Generic;
using System.Linq;
using Secyud.Ugf.Logging;
using Secyud.Ugf.Unity.TableComponents;
using Secyud.Ugf.Unity.TableComponents.Components;
using Secyud.Ugf.Unity.TableComponents.LocalComponents;
using Steamworks;
using UnityEngine;

namespace Secyud.Ugf.Steam.WorkshopManager
{
    public class LocalWorkshopManager : MonoBehaviour
    {
        private static List<WorkshopItemInfo> _localItems;

        public static List<WorkshopItemInfo> LocalItems =>
            _localItems ??= SearchLocalItem();

        private static List<WorkshopItemInfo> SearchLocalItem()
        {
            _ = SteamManager.Instance;

            _localItems ??= new List<WorkshopItemInfo>();

            uint itemNum = SteamUGC.GetNumSubscribedItems();
            PublishedFileId_t[] fileIds = new PublishedFileId_t[itemNum];
            SteamUGC.GetSubscribedItems(fileIds, itemNum);

            foreach (PublishedFileId_t fieldId in fileIds)
            {
                try
                {
                    WorkshopItemInfo itemInfo = LocalItems
                        .FirstOrDefault(u => u.Id == fieldId);

                    if (itemInfo is null)
                    {
                        itemInfo = new WorkshopItemInfo(fieldId);
                        _localItems.Add(itemInfo);
                    }

                    itemInfo.RefreshConfigInfo();
                }
                catch (Exception e)
                {
                    UgfLogger.LogError($"Mod load failed: fieldId-{fieldId}");
                    UgfLogger.LogError(e);
                }
            }

            return _localItems;
        }

        [SerializeField] private FilterInput _filterInput;
        [SerializeField] private FilterGroup _filterGroup;
        [SerializeField] private SorterGroup _sorterGroup;
        [SerializeField] private SingleSelect _singleSelect;
        [SerializeField] private Table _table;

        private void Awake()
        {
            WorkshopGlobalService service = U.Get<WorkshopGlobalService>();

            if (_table.Source is LocalTableSource tableSource)
            {
                tableSource.SourceGetter += () => LocalItems;
            }

            _table.InitLocalFilterInput(_filterInput, new WorkshopItemFilterName());
            _table.InitLocalFilterGroup(_filterGroup, service.FilterTags);
            _table.InitLocalSorterGroup(_sorterGroup,
                new LocalSorterBase[]
                {
                    new WorkshopItemSorterName()
                });
        }

        /// <summary>
        /// Unsubscribe the selected item and refresh table.
        /// </summary>
        public void UnsubscribeCurrentItem()
        {
            if (_singleSelect.SelectedCell.CellObject is
                WorkshopItemInfo info)
            {
                SteamUGC.UnsubscribeItem(info.Id);
                _localItems.Remove(info);
            }

            _table.Refresh(4);
        }

        /// <summary>
        /// Refresh local item info. It will reload all
        /// local workshop files.
        /// </summary>
        public void Refresh()
        {
            SearchLocalItem();
            _table.Refresh(4);
        }
    }
}