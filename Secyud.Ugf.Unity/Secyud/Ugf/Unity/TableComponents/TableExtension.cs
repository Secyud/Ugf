using System;
using System.Collections.Generic;
using System.Linq;
using Secyud.Ugf.Unity.TableComponents.LocalTable;
using Secyud.Ugf.Unity.TableComponents.UiFunctions;

namespace Secyud.Ugf.Unity.TableComponents
{
    public static class TableExtension
    {
        public static bool TrySetLocalSource(this Table table, Func<IEnumerable<object>> getter)
        {
            if (table.Source is LocalTableSource localTableSource)
            {
                localTableSource.SetSource(getter);
                return true;
            }

            return false;
        }
        
        
        public static void InitLocalFilterGroup<TFilter>(
            this Table table,
            FilterGroup group,
            IEnumerable<TFilter> filters)
            where TFilter : ILocalFilter, ITableFilterDescriptor
        {
            if (table.Filter is LocalTableFilter localFilter)
            {
                group.Initialize(filters);
                localFilter.FilterEvent.Add(Filter);
            }

            return;

            IEnumerable<object> Filter(IEnumerable<object> objects)
            {
                List<ILocalFilter> workedFilters = group
                    .GetWorkedFilters()
                    .Cast<ILocalFilter>().ToList();
                return objects
                    .Where(o => workedFilters
                        .Any(f => f.Filter(o)));
            }
        }

        public static void InitLocalFilterInput<TFilter>(
            this Table table,
            FilterInput input,
            TFilter filter)
            where TFilter : ILocalFilter, ITableStringFilterDescriptor
        {
            if (table.Filter is LocalTableFilter localFilter)
            {
                input.Initialize(filter);
                localFilter.FilterEvent .Add(Filter); 
            }

            return;

            IEnumerable<object> Filter(IEnumerable<object> objects)
            {
                return string.IsNullOrEmpty(filter.FilterString)
                    ? objects
                    : objects.Where(o => filter.Filter(o));
            }
        }

        public static void InitLocalSorterGroup<TSorter>(
            this Table table, SorterGroup group,
            IEnumerable<TSorter> sorters)
            where TSorter : ILocalSorter, ITableSorterDescriptor
        {
            if (table.Filter is LocalTableFilter localFilter)
            {
                group.Initialize(sorters);
                localFilter.FilterEvent.Add(Sorter); 
            }

            return;

            IEnumerable<object> Sorter(IEnumerable<object> objects)
            {
                List<ILocalSorter> workedSorters = group
                    .GetWorkedSorters()
                    .Cast<ILocalSorter>().ToList();

                List<object> result = objects.ToList();
                foreach (ILocalSorter sorter in workedSorters)
                {
                    result.Sort(sorter.Compare);
                }

                return result;
            }
        }

        public static void InitLocalSorterDropdown<TSorter>(
            this Table table, SorterDropdown dropdown,
            IEnumerable<TSorter> sorters)
            where TSorter : ILocalSorter, ITableSorterDescriptor
        {
            if (table.Filter is LocalTableFilter localFilter)
            {
                dropdown.Initialize(sorters);
                localFilter.FilterEvent.Add(Sorter); 
            }

            return;

            IEnumerable<object> Sorter(IEnumerable<object> objects)
            {
                if (dropdown.SelectedSorter is ILocalSorter workedSorter)
                {
                    List<object> result = objects.ToList();
                    result.Sort(workedSorter.Compare);
                    return result;
                }

                return objects.ToList();
            }
        }
    }
}