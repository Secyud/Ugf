using System;
using System.Collections.Generic;
using UnityEngine;

namespace Secyud.Ugf.TableComponents
{
    public abstract class TableDelegate : TableComponentDelegateBase<Table, TableDelegate>
    {
        public int IndexFirst { get; set; } = 0;
        public int IndexLast { get; set; } = 12;
        public abstract TableCell CreateCell(int index);
        public abstract void RefreshPrepare();
        protected TableDelegate(Table table) : base(table, table)
        {
        }
    }

    public class TableDelegate<TItem> : TableDelegate
    {
        private readonly TableCell _cellTemplate;
        public IList<TItem> Items { get; private set; }
        public IList<TItem> ItemsTmp { get; set; }

        private TableDelegate(Table table, IList<TItem> items, TableCell cellTemplate)
            : base(table)
        {
            _cellTemplate = cellTemplate;
            Items = items ?? new List<TItem>();
            ItemsTmp = items;
        }

        public static TableDelegate<TItem> Create(Table table, IList<TItem> items, TableCell cellTemplate)
        {
            return new TableDelegate<TItem>(table, items, cellTemplate);
        }

        private event Action<TableCell, TItem> CellInitAction;

        public void BindInitAction(Action<TableCell, TItem> action)
        {
            CellInitAction += action;
        }

        public void ClearInitAction()
        {
            CellInitAction = null;
        }

        public void OnCellInit(TableCell cell, TItem item)
        {
            CellInitAction?.Invoke(cell, item);
        }

        public override TableCell CreateCell(int index)
        {
            if (index >= ItemsTmp.Count)
                return null;
            TableCell cell = _cellTemplate.Instantiate(Component.Content);
            cell.transform.SetSiblingIndex(index);
            index += IndexFirst;
            cell.CellIndex = index;
            OnCellInit(cell, ItemsTmp[index]);
            return cell;
        }

        public override void RefreshPrepare()
        {
            ItemsTmp = Items;
        }
    }
}