using System;
using System.Collections.Generic;
using System.Linq;
using Secyud.Ugf.TableComponents.PagerComponents;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Secyud.Ugf.TableComponents.SorterComponents
{
    public abstract class SorterDelegate : TableComponentDelegateBase<Sorter, SorterDelegate>
    {
        public abstract void ApplySorter();

        protected SorterDelegate(Table table) : base(table, (Sorter)table[nameof(Sorter)])
        {
        }
    }

    public class SorterDelegate<TItem> : SorterDelegate
    {
        private readonly List<SorterToggleDescriptor<TItem>> _toggleDescriptors;

        public TableDelegate<TItem> TableDelegate => (TableDelegate<TItem>)Table.Delegate;

        private SorterDelegate(Table table, IEnumerable<SorterToggleDescriptor<TItem>> descriptors)
            : base(table)
        {
            _toggleDescriptors = descriptors.ToList();

            RectTransform layout = Component.Layout.RectTransform;

            for (int i = 0; i < layout.childCount; i++)
                Object.Destroy(layout.GetChild(i).gameObject);

            foreach (SorterToggleDescriptor<TItem> sorter in _toggleDescriptors)
                Component.CreateToggle(sorter);

            Component.Layout.enabled = true;
            
            table.AddRefreshAction(32, ApplySorter);
        }

        public override void ApplySorter()
        {
            if (_toggleDescriptors is null)
                return;

            IEnumerable<SorterToggleDescriptor<TItem>> sorters =
                _toggleDescriptors
                    .Where(u => u.Enabled != null)
                    .OrderBy(u => u.Position.GetSiblingIndex());

            TableDelegate.ItemsTmp = TableDelegate.ItemsTmp.SortBy(sorters).ToList();
        }

        public static SorterDelegate<TItem> Create<TService>(Table table)
            where TService : SorterRegeditBase<TItem>
        {
            return new SorterDelegate<TItem>(table, U.Get<TService>().Items);
        }

        public static SorterDelegate Create(Table table, IEnumerable<SorterToggleDescriptor<TItem>> descriptors)
        {
            return new SorterDelegate<TItem>(table, descriptors);
        }
    }
}