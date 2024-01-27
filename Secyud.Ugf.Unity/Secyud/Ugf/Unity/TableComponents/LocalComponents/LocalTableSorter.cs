using System;
using System.Collections.Generic;
using System.Linq;

namespace Secyud.Ugf.Unity.TableComponents.LocalComponents
{
    public class LocalTableSorter:TableDataOperator
    {
        public event Func<IEnumerable<object>, IEnumerable<object>> SorterEvent;
        public IList<object> SortedData { get; protected set; }
        public override void Apply()
        {
            if (Table.Filter is LocalTableFilter
                {
                    FilteredData: not null
                } localFilter)
            {
                SortedData =
                    (SorterEvent?.Invoke(localFilter.FilteredData)
                     ?? localFilter.FilteredData).ToList();
            }
        }
    }
}