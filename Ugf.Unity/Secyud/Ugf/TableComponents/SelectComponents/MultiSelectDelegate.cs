using System;
using System.Collections.Generic;
using Secyud.Ugf.BasicComponents;
using UnityEngine;

namespace Secyud.Ugf.TableComponents.SelectComponents
{
    public abstract class MultiSelectDelegate : TableComponentDelegateBase<MultiSelect, MultiSelectDelegate>
    {
        protected MultiSelectDelegate(Table table)
            : base(table, (MultiSelect)table[nameof(MultiSelect)])
        {
        }

        public abstract void OnEnsure();
    }

    public class MultiSelectDelegate<TItem> : MultiSelectDelegate
    {
        public IList<TItem> SelectedItems { get; }

        public TableDelegate<TItem> TableDelegate => (TableDelegate<TItem>)Table.Delegate;

        private MultiSelectDelegate(Table table, IList<TItem> bindList)
            : base(table)
        {
            TableDelegate.BindInitAction(InitCell);
            SelectedItems = bindList ?? new List<TItem>();
        }

        public static MultiSelectDelegate<TItem> Create(Table table, IList<TItem> bindList = null)
        {
            return new MultiSelectDelegate<TItem>(table, bindList);
        }

        private void InitCell(TableCell cell, TItem item)
        {
            SButton button = cell.gameObject.GetOrAddComponent<SButton>();
            button.onClick.AddListener(() => SelectItem(cell, item));
            SetSelect(cell, item, SelectedItems.Contains(item));
        }

        private void SelectItem(TableCell cell, TItem item)
        {
            bool selected = SelectedItems.Contains(item);
            if (selected)
                SelectedItems.Remove(item);
            else
                SelectedItems.Add(item);

            SetSelect(cell, item, !selected);
        }

        private event Action<TableCell, TItem, bool> SelectSetAction;

        public void BindSelectSetAction(Action<TableCell, TItem, bool> action)
        {
            SelectSetAction += action;
        }

        public void ClearSelectSetAction()
        {
            SelectSetAction = null;
        }

        public void SetSelect(TableCell cell, TItem item, bool select)
        {
            SelectSetAction?.Invoke(cell, item, select);
        }

        private event Action<IList<TItem>> EnsureAction;

        public void BindEnsureAction(Action<IList<TItem>> action)
        {
            EnsureAction += action;
        }

        public void ClearEnsureAction()
        {
            EnsureAction = null;
        }

        public override void OnEnsure()
        {
            EnsureAction?.Invoke(SelectedItems);
        }
    }
}