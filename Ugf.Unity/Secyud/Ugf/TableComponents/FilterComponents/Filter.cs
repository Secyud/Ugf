using System.Collections.Generic;
using Secyud.Ugf.LayoutComponents;
using Secyud.Ugf.TableComponents.SorterComponents;
using UnityEngine;

namespace Secyud.Ugf.TableComponents.FilterComponents
{
    public sealed class Filter : TableComponentBase<Filter,FilterDelegate>, IFilterGroup<FilterTrigger>
    {
        public override string Name => nameof(Filter);
        
        public FilterTrigger TriggerTemplate;
        public List<FilterTrigger> Filters { get; private set; }
        public FilterTrigger DroppedFilter { get; set; }
        public LayoutGroupTrigger Layout { get; private set; }

        private void Awake()
        {
            Layout = gameObject.GetOrAddComponent<LayoutGroupTrigger>();
            Filters = new List<FilterTrigger>();
        }

        public void LateUpdate()
        {
            enabled = false;
            if (Delegate is not null)
            {
                Delegate.Table.Delegate.RefreshPrepare();
                Delegate.ApplyFilter();
            }
        }

        public void RefreshTable()
        {
            enabled = true;
        }

        public void CreateTrigger<TItem>(FilterTriggerDescriptor<TItem> descriptor)
        {
            FilterTrigger trigger = TriggerTemplate.Create(transform, this, descriptor);
            Filters.Add(trigger);
            trigger.Registrations.AddRange(descriptor.Filters);
        }

        public static void CheckComponent(Table table)
        {
            Filter filter = (Filter)table[nameof(Filter)];

            if (filter is null)
                Sorter.CheckComponent(table);
            else
                table.AddRefreshAction(48,filter.Delegate.ApplyFilter);
        }
    }
}