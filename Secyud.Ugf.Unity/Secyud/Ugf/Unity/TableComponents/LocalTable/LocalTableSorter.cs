using System;
using System.Collections.Generic;
using System.Linq;

namespace Secyud.Ugf.Unity.TableComponents.LocalTable
{
    public class LocalTableSorter : TableDataOperator
    {
        public readonly List<Func<IEnumerable<object>, IEnumerable<object>>> SorterEvent = new();
        public IList<object> SortedData { get; protected set; }

        public override void Apply()
        {
            if (Table.Filter is LocalTableFilter
                {
                    FilteredData: not null
                } localFilter)
            {
                var source = localFilter.FilteredData;
                foreach (var func in SorterEvent)
                {
                    source = func.Invoke(source);
                }

                SortedData = source.ToList();
            }
        }
    }
}