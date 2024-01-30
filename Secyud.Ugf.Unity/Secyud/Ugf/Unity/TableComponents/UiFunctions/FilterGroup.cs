using System.Collections.Generic;
using System.Linq;
using Secyud.Ugf.Unity.Ui;
using UnityEngine;

namespace Secyud.Ugf.Unity.TableComponents.UiFunctions
{
    /// <summary>
    /// <para>
    /// The ui component of filter，use sub class of
    /// <see cref="TableOperator"/> to manage filter way.
    /// </para>
    /// <para>
    /// Local usage see
    /// <see cref="TableExtension.InitLocalFilterInput{TFilter}"/>
    /// </para>
    /// </summary>
    [RequireComponent(typeof(LayoutTrigger))]
    public class FilterGroup : MonoBehaviour
    {
        [SerializeField] private FilterToggle _toggleTemplate;
        [SerializeField] private TableOperator _filterFunction;
        private List<FilterToggle> _filterToggles;
        public LayoutTrigger LayoutTrigger { get; private set; }

        private void Awake()
        {
            LayoutTrigger = GetComponent<LayoutTrigger>();
            _filterToggles = new List<FilterToggle>();
        }

        public void Initialize<TFilter>(
            IEnumerable<TFilter> filters)
            where TFilter : ITableFilterDescriptor
        {
            foreach (TFilter filter in filters)
            {
                _filterToggles.Add(_toggleTemplate
                    .Instantiate(transform)
                    .Initialize(this, filter));
            }

            LayoutTrigger.Refresh();
        }

        public IList<ITableFilterDescriptor> GetWorkedFilters()
        {
            return _filterToggles
                .Where(toggle => toggle.IsOn)
                .Select(toggle => toggle.Filter)
                .ToList();
        }

        public void SetToggle(FilterToggle filterToggle)
        {
            bool value = _filterToggles
                .All(u => (filterToggle == u) ^ u.IsOn);

            foreach (FilterToggle toggle in _filterToggles)
            {
                toggle.IsOn = (filterToggle == toggle) ^ !value;
            }
        }

        public void Apply()
        {
            _filterFunction.Table.Refresh(3);
        }
    }
}