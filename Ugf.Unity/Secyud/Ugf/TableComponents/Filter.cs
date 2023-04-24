#region

using System.Linq;
using Secyud.Ugf.BasicComponents;
using Secyud.Ugf.ButtonComponents;
using UnityEngine;

#endregion

namespace Secyud.Ugf.TableComponents
{
    public class Filter : MonoBehaviour
    {
        [SerializeField] private SText Name;
        [SerializeField] private SDualToggle Toggle;
        private FilterGroup _filterGroup;

        public void OnLeftClick()
        {
            Toggle.isOn = !Toggle.isOn;
            Refresh();
        }

        public void OnRightClick()
        {
            var value = _filterGroup.Filters.All(u => (this == u) ^ u.Toggle.isOn);

            foreach (var filter in _filterGroup.Filters)
            {
                filter.Toggle.isOn = (this == filter) ^ !value;
                filter.Refresh();
            }
        }

        private void Refresh()
        {
            _filterGroup.FunctionalTable.RefreshFilter();
        }

        private void OnInitialize(FilterGroup filterGroup, ICanBeEnabled canBeEnabled)
        {
            _filterGroup = filterGroup;
            Toggle.Bind(canBeEnabled.SetEnabled);
            Name.text = Og.L[canBeEnabled.Name];
        }

        public Filter Create(Transform parent, FilterGroup filterGroup, ICanBeEnabled canBeEnabled)
        {
            var ret = Instantiate(this, parent);

            ret.OnInitialize(filterGroup, canBeEnabled);

            return ret;
        }
    }
}