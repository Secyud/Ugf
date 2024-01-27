using Secyud.Ugf.Unity.TableComponents.LocalTable;

namespace Secyud.Ugf.Steam.WorkshopManager
{
    public class WorkshopItemFilterTag:LocalFilterBase
    {
        public WorkshopItemFilterTag(string tagName) : base(tagName)
        {
        }
        
        public override bool Filter(object target)
        {
            return target is WorkshopItemInfo info &&
                   info.ConfigInfo.Tags.Contains(Name);
        }
    }
}