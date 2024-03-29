﻿using System;
using System.Collections.Generic;

namespace Secyud.Ugf.Unity.TableComponents.LocalTable
{
    public class LocalTableSorter : TableSorter
    {
        public readonly List<Func<IEnumerable<object>, IEnumerable<object>>> SorterEvent = new();
        public List<object> SortedData { get; } = new();

        public override void Apply()
        {
            if (Filter is LocalTableFilter
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
                SortedData.AddRange(source); 
            }
        }
    }
}