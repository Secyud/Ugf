using Secyud.Ugf.Unity.TableComponents.LocalTable;

namespace Secyud.Ugf.Steam.WorkshopManager
{
    public class WorkshopItemSorterName : LocalSorterBase
    {
        public override int Compare(object left, object right)
        {
            if (left is WorkshopItemInfo infoL &&
                right is WorkshopItemInfo infoR)
            {
                return string.CompareOrdinal(
                    infoL.ConfigInfo.Name,
                    infoR.ConfigInfo.Name);
            }

            return string.CompareOrdinal(
                left.ToString(),
                right.ToString());
        }

        public WorkshopItemSorterName() : base("Name")
        {
        }
    }
}