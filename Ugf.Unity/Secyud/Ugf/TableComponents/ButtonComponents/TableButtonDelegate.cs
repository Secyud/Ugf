using System.Collections.Generic;
using System.Linq;
using Secyud.Ugf.BasicComponents;
using UnityEngine;

namespace Secyud.Ugf.TableComponents.ButtonComponents
{
    public abstract class TableButtonDelegate : TableComponentDelegateBase<TableButton, TableButtonDelegate>
    {
        protected TableButtonDelegate(Table table)
            : base(table, (TableButton)table[nameof(TableButton)])
        {
        }
    }

    public class TableButtonDelegate<TItem> : TableButtonDelegate
    {
        private readonly List<ButtonDescriptor<TItem>> _descriptors;

        public TableDelegate<TItem> TableDelegate => (TableDelegate<TItem>)Table.Delegate;

        public TableButtonDelegate(Table table, IEnumerable<ButtonDescriptor<TItem>> descriptors) : base(table)
        {
            _descriptors = descriptors.ToList();
            TableDelegate.BindInitAction(InitCell);
        }

        public static TableButtonDelegate<TItem> Create(Table table, IEnumerable<ButtonDescriptor<TItem>> descriptors)
        {
            return new TableButtonDelegate<TItem>(table, descriptors);
        }

        public static TableButtonDelegate<TItem> Create<TButtonService>(Table table)
            where TButtonService : ButtonRegeditBase<TItem>
        {
            return new TableButtonDelegate<TItem>(table, U.Get<TButtonService>().Items);
        }

        private void InitCell(TableCell cell, TItem item)
        {
            SButton button = cell.gameObject.GetOrAddComponent<SButton>();
            button.onClick.AddListener(() => OnClick(item));
        }

        private void OnClick(TItem item)
        {
            Component.Template.Create(item, _descriptors.Where(u => u.Visible()));
        }
    }
}