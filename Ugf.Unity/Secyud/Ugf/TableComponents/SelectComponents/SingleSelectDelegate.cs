using System;
using System.Collections.Generic;
using Secyud.Ugf.BasicComponents;
using UnityEngine;

namespace Secyud.Ugf.TableComponents.SelectComponents
{
    public abstract class SingleSelectDelegate : TableComponentDelegateBase<SingleSelect, SingleSelectDelegate>
    {
        protected SingleSelectDelegate(Table table)
            : base(table, (SingleSelect)table[nameof(SingleSelect)])
        {
        }

        public abstract void OnEnsure();
    }

    public class SingleSelectDelegate<TItem> : SingleSelectDelegate
    where TItem:class
    {
        private TItem _selectedItem;

        public TableDelegate<TItem> TableDelegate => (TableDelegate<TItem>)Table.Delegate;

        private SingleSelectDelegate(Table table)
            : base(table)
        {
            TableDelegate.BindInitAction(InitCell);
        }

        public static SingleSelectDelegate<TItem> Create(Table table)
        {
            return new SingleSelectDelegate<TItem>(table);
        }

        private void InitCell(TableCell cell, TItem item)
        {
            SButton button = cell.gameObject.GetOrAddComponent<SButton>();
            button.onClick.AddListener(() => SelectItem(cell, item));
        }

        private void SelectItem(TableCell cell, TItem item)
        {
            _selectedItem = item == _selectedItem ? null : item;
                
            TableCell target = Component.Cell;
            SButton button = target.gameObject.GetOrAddComponent<SButton>();
            button.onClick.RemoveAllListeners();
            if (_selectedItem != null)
            {
                target.CellIndex = cell.CellIndex;
                TableDelegate.OnCellInit(cell, item);
            }
        }

        private event Action<TItem> EnsureAction;

        public void BindEnsureAction(Action<TItem> action)
        {
            EnsureAction += action;
        }

        public void ClearEnsureAction()
        {
            EnsureAction = null;
        }

        public override void OnEnsure()
        {
            EnsureAction?.Invoke(_selectedItem);
        }
    }
}