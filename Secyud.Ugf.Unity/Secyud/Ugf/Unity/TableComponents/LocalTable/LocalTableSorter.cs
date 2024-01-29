using System;
using System.Collections.Generic;

namespace Secyud.Ugf.Unity.TableComponents.LocalTable
{
    public class LocalTableSorter : TableDataOperator
    {
        public readonly List<Func<IEnumerable<object>, IEnumerable<object>>> SorterEvent = new();
        public List<object> SortedData { get; } = new();

        public override void Apply()
        {
            if (Table.Filter is LocalTableFilter
                {
                    FilteredData: not null
                } localFilter)
            {
                IEnumerable<object> source = localFilter.FilteredData;
                foreach (var func in SorterEvent)
                {
                    source = func.Invoke(source);
                }

                SortedData.Clear();
                SortedData.Add(source); 
            }
        }
    }
}