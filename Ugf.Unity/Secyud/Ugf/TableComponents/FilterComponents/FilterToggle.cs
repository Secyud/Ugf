#region

using UnityEngine;

#endregion

namespace Secyud.Ugf.TableComponents.FilterComponents
{
    public class FilterToggle : FilterBase<FilterToggle>
    {
        public FilterToggle Create(Transform parent, FilterTrigger filterTrigger, ICanBeEnabled canBeEnabled)
        {
            FilterToggle ret = Instantiate(this, parent);

            ret.SetFilter(filterTrigger, canBeEnabled);

            return ret;
        }
    }
}