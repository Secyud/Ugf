using Secyud.Ugf.Unity.TableComponents.Components;
using Secyud.Ugf.Unity.TableComponents.LocalComponents;

namespace Secyud.Ugf.Steam.WorkshopManager
{
    public class WorkshopItemFilterName:ILocalFilter,ITableStringFilterDescriptor
    {
        public bool Filter(object target)
        {
            return target is WorkshopItemInfo info &&
                   info.ConfigInfo.Name.Contains(FilterString);
        }

        public bool State { get; set; }
        public string FilterString { get; set; }
        public string Name => "Name";
    }
}