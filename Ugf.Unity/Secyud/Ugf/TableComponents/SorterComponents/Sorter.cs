using Secyud.Ugf.LayoutComponents;
using Secyud.Ugf.TableComponents.PagerComponents;
using UnityEngine;

namespace Secyud.Ugf.TableComponents.SorterComponents
{
    public sealed class Sorter : TableComponentBase<Sorter, SorterDelegate>
    {
        public override string Name => nameof(Sorter);

        public SorterToggle ToggleTemplate;
        
        private LayoutGroupTrigger _layout;
        public LayoutGroupTrigger Layout => _layout ??= gameObject.GetOrAddComponent<LayoutGroupTrigger>();

        public void RefreshTable()
        {
            Delegate.Table.Refresh();
        }

        public void CreateToggle<TItem>(SorterToggleDescriptor<TItem> descriptor)
        {
            SorterToggle toggle = ToggleTemplate.Create(transform, this, descriptor);
            descriptor.Position = toggle.transform;
        }
    }
}