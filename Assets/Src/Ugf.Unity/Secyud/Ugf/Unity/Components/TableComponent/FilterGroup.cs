#region

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#endregion

namespace Secyud.Ugf.Unity.Components
{
    public class FilterGroup : MonoBehaviour
    {
        [SerializeField] private SText Name;
        [SerializeField] private SDualToggle Toggle;
        [SerializeField] private Filter FilterTemplate;
        [SerializeField] private SFloating FloatingTemplate;

        private SFloating _floatingExist;
        public FunctionalTable FunctionalTable { get; private set; }
        public List<ICanBeEnabled> ChildFilters { get; private set; }
        public List<Filter> Filters { get; private set; }

        public bool IsOn => Toggle.isOn;

        public bool IsDropped
        {
            get => _floatingExist;
            set
            {
                if (value)
                {
                    foreach (var group in FunctionalTable.FilterGroups)
                        group.IsDropped = false;

                    _floatingExist = FloatingTemplate.CreateOnMouse();

                    Filters.Clear();

                    foreach (ICanBeEnabled filter in ChildFilters)
                    {
                        Filters.Add(FilterTemplate.Create(_floatingExist.Content, this, filter));
                    }

                    _floatingExist.CheckBoundary();
                }
                else if (_floatingExist)
                    CloseFlow();
            }
        }

        private void Awake()
        {
            ChildFilters = new();
            Filters = new();
        }

        public void OnLeftClick()
        {
            Toggle.isOn = !Toggle.isOn;
            Refresh();
        }

        public void OnRightClick()
        {
            bool value = FunctionalTable.FilterGroups.All(u => (this == u) ^ u.Toggle.isOn);

            foreach (var filterGroup in FunctionalTable.FilterGroups)
            {
                filterGroup.Toggle.isOn = (this == filterGroup) ^ !value;
                filterGroup.Refresh();
            }
        }

        public void OnClick()
        {
            IsDropped = !IsDropped;
        }

        private void Refresh()
        {
            FunctionalTable.RefreshFilter();
        }

        private void CloseFlow()
        {
            Destroy(_floatingExist.gameObject);
            _floatingExist = null;
            Filters.Clear();
        }

        private void OnInitialize(FunctionalTable functionalManager, ICanBeEnabled canBeEnabled)
        {
            FunctionalTable = functionalManager;
            Toggle.Bind(canBeEnabled.SetEnabled);
            Name.text = Og.L[canBeEnabled.Name];
        }

        public FilterGroup Create(Transform parent, FunctionalTable functionalTable, ICanBeEnabled canBeEnabled)
        {
            var ret = Instantiate(this, parent);

            ret.OnInitialize(functionalTable, canBeEnabled);

            return ret;
        }
    }
}