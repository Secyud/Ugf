using System.Collections.Generic;
using Secyud.Ugf.LayoutComponents;
using UnityEngine;

namespace Secyud.Ugf.TableComponents.FilterComponents
{
    public sealed class Filter : TableComponentBase<Filter, FilterDelegate>, IFilterGroup<FilterTrigger>
    {
        public override string Name => nameof(Filter);

        public FilterTrigger TriggerTemplate;
        private LayoutGroupTrigger _layout;
        public FilterTrigger DroppedFilter { get; set; }
        public List<FilterTrigger> Filters { get; } = new();

        public LayoutGroupTrigger Layout => _layout ??= gameObject.GetOrAddComponent<LayoutGroupTrigger>();

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
            CreateTrigger(descriptor, descriptor.Filters);
        }
        public void CreateTrigger(ICanBeEnabled descriptor,IEnumerable<ICanBeEnabled> filters)
        {
            FilterTrigger trigger = TriggerTemplate.Create(transform, this, descriptor);
            Filters.Add(trigger);
            trigger.Registrations.AddRange(filters);
        }
    }
}