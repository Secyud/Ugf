using System;

namespace Secyud.Ugf.TableComponents
{
    public abstract class TableComponentDelegateBase<TTableComponent,TTableComponentDelegate>
    where TTableComponentDelegate : TableComponentDelegateBase<TTableComponent, TTableComponentDelegate>
    where TTableComponent:TableComponentBase<TTableComponent,TTableComponentDelegate>
    {
        public Table Table { get; }
        public TTableComponent Component { get; }

        protected TableComponentDelegateBase(Table table,TTableComponent component)
        {
            Table = table;
            Component = component ? component :
                throw new ArgumentNullException(nameof(component) +
                $" count be null. (in {GetType()})");
            Component.Delegate = (TTableComponentDelegate)this;
        }
    }
}