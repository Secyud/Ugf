using System;
using System.Collections.Generic;
using System.Linq;
using Secyud.Ugf.Unity.TableComponents.LocalTable;
using Secyud.Ugf.Unity.TableComponents.UiFunctions;
using UnityEngine;
using UnityEngine.Pool;

namespace Secyud.Ugf.Unity.TableComponents
{
    public static class TableExtension
    {
        public static MultiSelect SetMultiSelectEvent(this Table table, Action<object, bool> action)
        {
            var ret = table.GetOrAddComponent<MultiSelect>();
            ret.SelectChangedEvent += action;
            return ret;
        }

        public static SingleSelect SetSingleSelectEvent(this Table table, Action<object, bool> action)
        {
            var ret = table.GetOrAddComponent<SingleSelect>();
            ret.SelectChangedEvent += action;
            return ret;
        }

        public static void SetLocalSource(this Table table, Func<IEnumerable<object>> getter)
        {
            table.GetComponent<LocalTableSource>().SetSource(getter);
        }


        public static void InitTableButton(this Table table,
            IEnumerable<ITableButtonDescriptor> buttons)
        {
            table.GetComponent<TableButton>().Initialize(buttons);
        }

        public static void InitLocalFilterGroup<TFilter>(
            this Table table,
            FilterGroup group,
            IEnumerable<TFilter> filters)
            where TFilter : ILocalFilterDescriptor, ITableFilterDescriptor
        {
            group.Initialize(filters);
            table.GetComponent<LocalTableFilter>()
                .FilterEvent.Add(Filter);

            return;

            IEnumerable<object> Filter(IEnumerable<object> objects)
            {
                List<ILocalFilterDescriptor> workedFilters =
                    ListPool<ILocalFilterDescriptor>.Get();

                foreach (var tableFilter in group.GetWorkedFilters())
                {
                    if (tableFilter is ILocalFilterDescriptor filter)
                    {
                        workedFilters.Add(filter);
                    }
                }

                List<object> ret = objects
                    .Where(o => workedFilters
                        .Any(filter => filter.Filter(o))).ToList();
                ListPool<ILocalFilterDescriptor>.Release(workedFilters);
                return ret;
            }
        }

        public static void InitLocalFilterInput<TFilter>(
            this Table table, FilterInput input, TFilter filter)
            where TFilter : ILocalFilterDescriptor, IFilterStringDescriptor
        {
            input.Initialize(filter);
            table.GetComponent<LocalTableFilter>()
                .FilterEvent.Add(Filter);
            return;

            IEnumerable<object> Filter(IEnumerable<object> objects)
            {
                if (input.Filter is not ILocalFilterDescriptor localFilter ||
                    string.IsNullOrEmpty(input.Filter?.FilterString))
                    return objects;
                return objects.Where(o => localFilter.Filter(o));
            }
        }


        public static void InitLocalSorterGroup<TSorter>(
            this Table table, SorterGroup group,
            IEnumerable<TSorter> sorters)
            where TSorter : ILocalSorterDescriptor, ITableSorterDescriptor
        {
            group.Initialize(sorters);
            table.GetComponent<LocalTableFilter>()
                .FilterEvent.Add(Sorter);

            return;

            IEnumerable<object> Sorter(IEnumerable<object> objects)
            {
                IEnumerable<object> result = objects;

                foreach (var tableSorter in group.GetWorkedSorters())
                {
                    if (tableSorter is ILocalSorterDescriptor sorter)
                    {
                        result = result.OrderBy(
                            o => sorter.GetSortValue(o));
                    }
                }

                return result;
            }
        }

        public static void InitLocalSorterDropdown<TSorter>(
            this Table table, SorterDropdown dropdown,
            IEnumerable<TSorter> sorters)
            where TSorter : ILocalSorterDescriptor, ITableSorterDescriptor
        {
            dropdown.Initialize(sorters);
            table.GetComponent<LocalTableFilter>()
                .FilterEvent.Add(Sorter);
            return;

            IEnumerable<object> Sorter(IEnumerable<object> objects)
            {
                if (dropdown.SelectedSorter is
                    ILocalSorterDescriptor workedSorter)
                {
                    return objects.OrderBy(
                        o => workedSorter.GetSortValue(o));
                }

                return objects;
            }
        }
    }
}