using Secyud.Ugf.LayoutComponents;
using Secyud.Ugf.TableComponents.PagerComponents;
using UnityEngine;

namespace Secyud.Ugf.TableComponents.SorterComponents
{
    public sealed class Sorter : TableComponentBase< Sorter,  SorterDelegate>
    {
        public override string Name => nameof(Sorter);

        public SorterToggle ToggleTemplate;
        public LayoutGroupTrigger Layout { get; private set; }

        private void Awake()
        {
            Layout = gameObject.GetOrAddComponent<LayoutGroupTrigger>();
        }

        public void LateUpdate()
        {
            enabled = false;
            Delegate?.ApplySorter();
        }

        public void RefreshTable(bool resetIndex = false)
        {
            enabled = true;
            if (resetIndex)
                Layout.enabled = true;
        }

        public void CreateToggle<TItem>(SorterToggleDescriptor<TItem> descriptor)
        {
            SorterToggle toggle = ToggleTemplate.Create(transform, this, descriptor);
            descriptor.Position = toggle.transform;
        }

        public static void CheckComponent(Table table)
        {
            Sorter sorter = (Sorter)table[nameof(Sorter)];

            if (sorter is null)
                ;
            else
                table.AddRefreshAction(32, sorter.Delegate.ApplySorter);
        }
    }
}