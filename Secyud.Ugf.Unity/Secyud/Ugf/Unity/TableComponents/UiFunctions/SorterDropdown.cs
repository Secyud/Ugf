using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace Secyud.Ugf.Unity.TableComponents.UiFunctions
{
    public class SorterDropdown : MonoBehaviour
    {
        [SerializeField] private TableDataOperator _filterFunction;
        [SerializeField] private TMP_Dropdown _dropdown;

        private List<ITableSorterDescriptor> _sorters;

        public ITableSorterDescriptor SelectedSorter { get; private set; }

        private void Awake()
        {
            _dropdown.onValueChanged.AddListener(SubmitInput);
        }

        public void Initialize<TSorter>(
            IEnumerable<TSorter> sorters)
            where TSorter:ITableSorterDescriptor
        {
            _sorters = sorters.Cast<ITableSorterDescriptor>().ToList();
            _dropdown.options = _sorters
                .Select(u => new TMP_Dropdown.OptionData(U.T[u.Name]))
                .ToList();
            _dropdown.SetValueWithoutNotify(0);
        }

        private void SubmitInput(int select)
        {
            SelectedSorter = _sorters[select];
            _filterFunction.Table.Refresh(3);
        }
    }
}