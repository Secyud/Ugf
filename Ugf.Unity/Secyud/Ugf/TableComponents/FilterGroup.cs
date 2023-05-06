#region

using System.Collections.Generic;
using System.Linq;
using Secyud.Ugf.BasicComponents;
using Secyud.Ugf.ButtonComponents;
using UnityEngine;

#endregion

namespace Secyud.Ugf.TableComponents
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

                    RectTransform content = _floatingExist.PrepareLayout();
                    
                    foreach (var filter in ChildFilters)
                        Filters.Add(FilterTemplate.Create(content, this, filter));
                }
                else if (_floatingExist)
                {
                    CloseFlow();
                }
            }
        }

        private void Awake()
        {
            ChildFilters = new List<ICanBeEnabled>();
            Filters = new List<Filter>();
        }

        public void OnLeftClick()
        {
            Toggle.isOn = !Toggle.isOn;
            Refresh();
        }

        public void OnRightClick()
        {
            var value = FunctionalTable.FilterGroups.All(u => (this == u) ^ u.Toggle.isOn);

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
            Name.text = Og.L[canBeEnabled.ShowName];
        }

        public FilterGroup Create(Transform parent, FunctionalTable functionalTable, ICanBeEnabled canBeEnabled)
        {
            var ret = Instantiate(this, parent);

            ret.OnInitialize(functionalTable, canBeEnabled);

            return ret;
        }
    }
}