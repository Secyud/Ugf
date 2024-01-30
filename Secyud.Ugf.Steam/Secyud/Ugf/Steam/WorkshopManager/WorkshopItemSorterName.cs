using Secyud.Ugf.Unity.TableComponents.LocalTable;

namespace Secyud.Ugf.Steam.WorkshopManager
{
    public class WorkshopItemSorterName : LocalSorterBase
    {
        public override object GetSortValue(object obj)
        {
            if (obj is WorkshopItemInfo info)
            {
                return info.ConfigInfo.Name;
            }

            return string.Empty;
        }

        public WorkshopItemSorterName() : base("Name")
        {
        }
    }
}