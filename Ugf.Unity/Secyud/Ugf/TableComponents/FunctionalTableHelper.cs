#region

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

#endregion

namespace Secyud.Ugf.TableComponents
{
    public abstract class FunctionalTableHelper<TItem, TCell, TListService> : TableHelper<TItem, TCell>
        where TListService : TableFunctionBase<TItem>
        where TCell : MonoBehaviour
    {
        protected IList<TItem> FilteredItems;
        protected List<FilterRegistrationGroup<TItem>> FilterGroups;
        protected List<Pair<ISorterRegistration<TItem>, Transform>> Sorters;
        protected IList<TItem> TotalItems;

        public void OnInitialize(FunctionalTable table, TCell cellTemplate, IList<TItem> showItems)
        {
            TotalItems = showItems;
            FilteredItems = TotalItems;

            var service = Og.Get<TListService>();

            FilterGroups = service.FilterGroups.ToList();

            for (var i = 0; i < table.FixedContent.childCount; i++)
                Object.Destroy(table.FixedContent.GetChild(i).gameObject);

            foreach (var filterGroup in FilterGroups)
            {
                var fgc = table.FilterGroupTemplate.Create(table.FixedContent, table, filterGroup);
                table.FilterGroups.Add(fgc);
                fgc.ChildFilters.AddRange(filterGroup.Filters);
            }

            Sorters = new List<Pair<ISorterRegistration<TItem>, Transform>>();

            for (var i = 0; i < table.SortableContent.childCount; i++)
                Object.Destroy(table.SortableContent.GetChild(i).gameObject);

            foreach (var sorter in service.Sorters)
            {
                var s = table.SorterTemplate.Create(table.SortableContent, table, sorter);

                Sorters.Add(new Pair<ISorterRegistration<TItem>, Transform>
                {
                    First = sorter,
                    Second = s.transform
                });
            }

            base.OnInitialize(table, cellTemplate, showItems);
        }

        public override void ApplyFilter()
        {
            if (FilterGroups is null) return;

            var filterGroups =
                FilterGroups
                    .Where(u => u.Enabled)
                    .Select(u =>
                        u.Filters.Where(v => v.Enabled));

            FilteredItems = TotalItems.AndOrFilterBy(filterGroups).ToList();
        }

        public override void ApplySorter()
        {
            if (Sorters is null) return;

            var sorters =
                Sorters
                    .Where(u => u.First.Enabled != null)
                    .OrderBy(u => u.Second.GetSiblingIndex())
                    .Select(u => new Pair<ISorterRegistration<TItem>, bool>
                    {
                        First = u.First,
                        Second = u.First.Enabled != null && u.First.Enabled.Value
                    });

            Items = FilteredItems.SortBy(sorters).ToList();
        }
    }
}