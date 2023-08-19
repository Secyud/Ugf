#region

using System.Collections;
using System.Collections.Generic;
using System.Ugf.Collections.Generic;

#endregion

namespace Secyud.Ugf.TableComponents.PagerComponents
{
    public abstract class PagerDelegate : TableComponentDelegateBase<Pager, PagerDelegate>
    {
        public abstract int Count { get; }
        public abstract void ApplyPager();
        protected PagerDelegate(Table table) : base(table, (Pager)table[nameof(Pager)])
        {
        }
    }

    public class PagerDelegate<TItem> : PagerDelegate, IList<TItem>
    {
        public TableDelegate<TItem> TableDelegate => (TableDelegate<TItem>)Table.Delegate;
        public IList<TItem> Items => TableDelegate.ItemsTmp;
        public override int Count => TableDelegate.ItemsTmp.Count;

        public bool IsReadOnly => TableDelegate.ItemsTmp.IsReadOnly;
        
        private PagerDelegate(Table table)
            : base(table)
        {
            table.SetCellCount(Component.PageSize);
        }

        public static PagerDelegate Create(Table table)
        {
            return new PagerDelegate<TItem>(table);
        }

        public int IndexOf(TItem item)
        {
            return Items.IndexOf(item);
        }

        public void Insert(int index, TItem item)
        {
            Items.Insert(index, item);
            Table.InsertAt(index);
        }

        public void RemoveAt(int index)
        {
            Items.RemoveAt(index);
            Table.RemoveAt(index);
        }

        public TItem this[int index]
        {
            get => Items[index];
            set => Items[index] = value;
        }

        public IEnumerator<TItem> GetEnumerator()
        {
            return Items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(TItem item)
        {
            Items.AddLast(item);
            Table.InsertAt(Items.Count - 1);
        }

        public void Clear()
        {
            Items.Clear();
            Table.Clear();
        }

        public bool Contains(TItem item)
        {
            return Items.Contains(item);
        }

        public void CopyTo(TItem[] array, int arrayIndex)
        {
            Items.CopyTo(array, arrayIndex);
            for (int i = arrayIndex; i < array.Length + arrayIndex; i++) Table.ReplaceAt(i);
        }

        public bool Remove(TItem item)
        {
            int index = Items.IndexOf(item);
            if (index < 0) return false;

            RemoveAt(index);
            return true;
        }

        public override void ApplyPager()
        {
            Component.RefreshPage();
        }
    }
}