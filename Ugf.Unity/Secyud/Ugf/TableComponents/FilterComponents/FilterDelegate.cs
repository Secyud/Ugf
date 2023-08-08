using System.Collections.Generic;
using System.Linq;
using Secyud.Ugf.TableComponents.PagerComponents;
using Secyud.Ugf.TableComponents.SorterComponents;
using Object = UnityEngine.Object;

namespace Secyud.Ugf.TableComponents.FilterComponents
{
    public abstract class FilterDelegate : TableComponentDelegateBase<Filter, FilterDelegate>
    {
        public abstract void ApplyFilter();

        protected FilterDelegate(Table table) : base(table, (Filter)table[nameof(Filter)])
        {
        }
    }

    public class FilterDelegate<TItem> : FilterDelegate
    {
        private readonly List<FilterTriggerDescriptor<TItem>> _triggerDescriptors;

        public TableDelegate<TItem> TableDelegate => (TableDelegate<TItem>)Table.Delegate;
        private FilterDelegate(Table table,IEnumerable<FilterTriggerDescriptor<TItem>> descriptors)
            : base(table)
        {
            _triggerDescriptors = descriptors.ToList();

            for (int i = 0; i < Component.transform.childCount; i++)
                Object.Destroy(Component.transform.GetChild(i).gameObject);

            foreach (FilterTriggerDescriptor<TItem> descriptor in _triggerDescriptors)
                Component.CreateTrigger(descriptor);

            Component.Layout.enabled = true;
        }
        
        public override void ApplyFilter()
        {
            IEnumerable<IEnumerable<FilterToggleDescriptor<TItem>>> filterGroups =
                _triggerDescriptors
                    .Where(u => u.GetEnabled())
                    .Select(
                        u =>
                            u.Filters.Where(v => v.GetEnabled())
                    );

            TableDelegate.ItemsTmp = TableDelegate.ItemsTmp.AndOrFilterBy(filterGroups).ToList();

            Sorter.CheckComponent(Table);
        }
        
        public static FilterDelegate<TItem> Create<TService>(Table table)
            where TService : FilterRegeditBase<TItem>
        {
            return new FilterDelegate<TItem>(table, U.Get<TService>().Items);
        }

        public static FilterDelegate Create(Table table, IEnumerable<FilterTriggerDescriptor<TItem>> descriptors)
        {
            return new FilterDelegate<TItem>(table, descriptors);
        }
    }
}